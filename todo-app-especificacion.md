# Especificación — TODO App semanal

## Objetivo

Aplicación simple de lista de tareas ("compromisos de la semana"): descripciones
breves con checkbox que marca pendiente/terminada. Sin autenticación. La
iteración 1 quedó en memoria; la iteración 2 (ver más abajo) migra la
persistencia a un archivo JSON en el backend — no se justifica una base de
datos para el volumen de tareas que maneja esta app.

Sirve como proyecto de prueba para el paradigma de "looping" (`/goal`) en
Claude Code, con Jira + GitHub como backlog y repositorio, vía MCP.

## Alcance

**Incluye (iteración 1, ya entregada):**
- Ver lista de tareas de la semana
- Agregar una tarea nueva (descripción breve)
- Marcar/desmarcar una tarea como terminada (checkbox)
- Borrar una tarea

**Incluye (iteración 2, ver sección dedicada más abajo):**
- Persistencia en archivo JSON (reemplaza memoria)
- Navegación entre semanas + volver a la semana actual
- Pasar tareas pendientes de una semana a la siguiente
- Filtros: todas / pendientes / completadas
- Prioridad por tarea (alta/media/baja)
- Modal para agregar tarea (reemplaza formulario inline)
- Modal de confirmación al eliminar

**No incluye (fuera de alcance):**
- Autenticación / usuarios
- Base de datos
- Edición de la descripción de una tarea ya creada
- Fechas límite específicas, categorías

## Estructura del repositorio (monorepo)

```
todo-app/
├── backend/        (ASP.NET Core Web API)
├── frontend/        (Angular)
└── README.md
```

## Stack

- **Backend:** ASP.NET Core Web API (.NET), persistencia en archivo JSON (ver Iteración 2)
- **Frontend:** Angular

## Modelo de datos

Modelo de la iteración 1 (ver sección "Iteración 2" más abajo para los
campos `Priority` y `WeekStart` agregados después):

```
TaskItem
├── Id: int
├── Description: string
└── Completed: bool
```

## Backend — endpoints

| Endpoint | Descripción |
|---|---|
| `GET /api/health` | Healthcheck, responde 200 |
| `GET /api/tasks` | Lista todas las tareas |
| `POST /api/tasks` | Crea una tarea nueva (`{ description }`) |
| `PUT /api/tasks/{id}` | Togglea `Completed` de una tarea existente |
| `DELETE /api/tasks/{id}` | Elimina una tarea existente |

CORS habilitado para permitir consumo desde `localhost:4200` (Angular en dev).

## Frontend — comportamiento

- Al cargar, consulta `GET /api/tasks` y muestra la lista: descripción + checkbox.
- Un formulario simple permite escribir una descripción y agregarla (`POST /api/tasks`).
- Al hacer click en un checkbox, se llama `PUT /api/tasks/{id}` y la vista refleja
  el nuevo estado (tachado o similar para "terminada").
- Cada tarea tiene un botón/ícono de eliminar; al hacer click llama
  `DELETE /api/tasks/{id}` y la tarea desaparece de la lista.

## Backlog — historias para Jira

Épica: **TODO App semanal**. Orden sugerido (el frontend depende de que el
backend ya exponga los endpoints correspondientes):

| # | Historia | Criterio de aceptación (check automático) |
|---|---|---|
| 1 | Setup API mínima + `GET /api/health` | `dotnet build` y `dotnet test` salen 0 en `/backend` |
| 2 | Modelo `TaskItem` + `GET /api/tasks` (en memoria) | Test de integración: 200 y JSON con lista |
| 3 | `POST /api/tasks` — crear tarea | Test: 201 + tarea aparece en `GET` |
| 4 | `PUT /api/tasks/{id}` — togglear completado | Test: 200 si existe, 404 si no |
| 5 | `DELETE /api/tasks/{id}` — eliminar tarea | Test: 200/204 si existe y desaparece de `GET`, 404 si no |
| 6 | CORS habilitado para Angular local | Request desde `localhost:4200` no es bloqueado |
| 7 | Setup Angular + `TaskService` (get/post/put/delete) | `ng build` sin errores en `/frontend` |
| 8 | Componente lista: descripción + checkbox | `ng test --watch=false` pasa, checkbox refleja estado |
| 9 | Click en checkbox → `PUT` y refresca vista | Test de componente simulando el click |
| 10 | Formulario para agregar tarea nueva | Test de componente + `ng build` sin errores |
| 11 | Botón eliminar tarea → `DELETE` y refresca vista | Test de componente simulando el click de borrado |

Cada issue debe incluir su criterio de aceptación en la descripción, para que
el loop sepa exactamente qué comando correr para validarlo.

## Iteración 2 — mejoras de UI, semanas y persistencia en JSON

A partir de la maquetación revisada, se agregan estos requerimientos sobre
la app ya entregada en la iteración 1.

### Modelo de datos (actualizado)

```
TaskItem
├── Id: int
├── Description: string
├── Completed: bool
├── Priority: string   ("alta" | "media" | "baja")
└── WeekStart: date     (lunes de la semana ISO a la que pertenece la tarea)
```

### Persistencia

El backend deja de guardar las tareas en memoria y las persiste en un
archivo `tasks.json` en disco (por ejemplo `backend/data/tasks.json`).
No se usa base de datos — el volumen esperado (tareas de una semana a la
vez) no lo justifica. Cada mutación (crear, togglear, borrar, carry-over)
se escribe inmediatamente al archivo; al arrancar, el backend carga el
archivo si existe o lo crea vacío si no.

### Backend — endpoints (actualizado)

| Endpoint | Descripción |
|---|---|
| `GET /api/health` | Healthcheck, responde 200 |
| `GET /api/tasks?week={YYYY-MM-DD}` | Lista tareas de la semana cuyo lunes es la fecha dada |
| `POST /api/tasks` | Crea tarea (`{ description, priority, weekStart }`) |
| `PUT /api/tasks/{id}` | Togglea `Completed` |
| `DELETE /api/tasks/{id}` | Elimina una tarea |
| `POST /api/tasks/carry-over?week={YYYY-MM-DD}` | Mueve las tareas pendientes de esa semana a la siguiente (actualiza su `WeekStart`) |

### Frontend — comportamiento nuevo

> Referencia visual obligatoria: `design-notes.md` (colores, tipografía,
> layout y detalle de cada componente extraídos de la maqueta). Debe
> vivir en la raíz del repo — el loop de ejecución lo necesita para las
> historias 16-22.

- Encabezado con navegación: botones semana anterior/siguiente, etiqueta de
  la semana (ej. "3-9 nov") y badge "Semana actual" cuando corresponde.
  Botón "Ir a hoy" que regresa a la semana en curso.
- Contador de completadas (`{{ doneCount }}/{{ totalCount }}`) con barra de
  progreso.
- Filtros tipo pill: Todas / Pendientes / Completadas, sobre la lista visible.
- "Nueva tarea" abre un modal (ya no formulario inline) con campo de
  descripción y selector de prioridad (alta/media/baja).
- Botón "Pasar pendientes a la próxima semana" que llama al endpoint de
  carry-over.
- Al eliminar una tarea, se pide confirmación en un modal antes de llamar
  `DELETE`.
- Estado vacío con mensaje cuando la semana/filtro no tiene tareas que mostrar.

### Backlog — historias para Jira (iteración 2)

| # | Historia | Criterio de aceptación (check automático) |
|---|---|---|
| 12 | Migrar persistencia de memoria a archivo `tasks.json` | Test: reiniciar el proceso y las tareas previas siguen en `GET /api/tasks` |
| 13 | Agregar campo `Priority` al modelo + `POST /api/tasks` lo acepta | Test: `POST` con priority inválida devuelve 400, válida devuelve 201 |
| 14 | Agregar campo `WeekStart` + `GET /api/tasks?week=` filtra por semana | Test: tareas de otras semanas no aparecen en la respuesta |
| 15 | `POST /api/tasks/carry-over?week=` — mueve pendientes a la semana siguiente | Test: tarea pendiente cambia de `WeekStart`, tarea completada no se mueve |
| 16 | Navegación entre semanas (prev/next) + "Ir a hoy" en el frontend | Test de componente: click en next llama `GET` con la semana siguiente |
| 17 | Contador de completadas + barra de progreso | Test de componente: refleja `doneCount`/`totalCount` correctos |
| 18 | Filtros Todas/Pendientes/Completadas | Test de componente: cada filtro muestra el subconjunto correcto |
| 19 | Modal "Nueva tarea" con selector de prioridad (reemplaza formulario inline) | Test de componente: abre, envía `POST` con priority, cierra al confirmar |
| 20 | Modal de confirmación al eliminar tarea | Test de componente: `DELETE` solo se llama tras confirmar en el modal |
| 21 | Botón "Pasar pendientes a la próxima semana" en frontend | Test de componente: llama al endpoint de carry-over y refresca la lista |
| 22 | Estado vacío (semana o filtro sin tareas) | Test de componente: muestra mensaje cuando la lista visible está vacía |

Igual que en la iteración 1: cada issue necesita su criterio de aceptación
en la descripción antes de pasar por el loop de refinamiento.

## Fase intermedia — afinar especificación de cada issue (loop de refinamiento)

Antes de lanzar el loop de desarrollo, cada issue del backlog debe pasar por
una "Definition of Ready": secciones obligatorias en su descripción de Jira
que lo hacen verificable y ejecutable sin ambigüedad. Este es un loop
independiente del loop de desarrollo — refina texto, no código.

### Definition of Ready (por issue)

La descripción de cada issue debe incluir estas secciones:

- **Contexto** — 1-3 líneas de por qué existe este issue
- **Criterio de aceptación** — lista concreta de condiciones (bullets o Given/When/Then)
- **Comando de verificación** — el comando exacto y el resultado exitoso esperado
  (ej. "`dotnet test` en `/backend`, debe salir 0")
- **Dependencias** — de qué otros issues depende, si aplica
- **Fuera de alcance** — qué NO cubre este issue, si aplica

Cuando las cinco están completas y son concretas, se agrega el label
`spec-ready`. El loop de desarrollo (fase de ejecución) solo debe tomar
issues con ese label.

### Check objetivo

`check_issue_ready.py` (ver archivo adjunto) valida el **texto** de una
descripción contra la Definition of Ready: verifica que las 3 secciones
obligatorias existan y tengan contenido no trivial, y que el comando de
verificación esté en la lista de comandos válidos conocidos (`dotnet build`,
`dotnet test`, `ng build`, `ng test --watch=false`). No se conecta a Jira —
el fetch del issue y el agregar el label `spec-ready` los hace el **mcp-jira**
dentro del loop; el script solo dice si el texto ya cumple o qué le falta.

Uso: `python3 check_issue_ready.py descripcion.md` (o `-` para leer de stdin).
Sale con código 0 si está listo, 1 si falta algo (con el detalle impreso).

### Comando `/goal` — refinamiento

```
/goal Afina la descripción de cada issue del proyecto LDT que no
tenga el label "spec-ready" (usa mcp-jira para leer y actualizar issues).
Usa todo-app-especificacion.md como contexto general del proyecto. Para
cada issue: redacta su descripción incluyendo las secciones Contexto,
Criterio de aceptación y Comando de verificación (agrega Dependencias y
Fuera de alcance si aplican). Guarda ese texto en un archivo temporal y
corre `python3 check_issue_ready.py <archivo>`. Si el script dice LISTO,
actualiza la descripción del issue en Jira vía mcp-jira y agrégale el
label spec-ready. Si dice NO LISTO, corrige el texto según lo que reporte
y vuelve a correr el script antes de escribir a Jira. No cambies el
título del issue ni crees issues nuevos. Detente cuando todos los issues
del proyecto tengan el label spec-ready, o tras 30 turnos.
```

**Checkpoint humano:** antes de pasar a la fase de ejecución, revisa tú
mismo los issues refinados — este loop garantiza que la *forma* esté
completa, no que el *criterio de aceptación* sea el correcto. Es una
revisión rápida (ya no de redacción desde cero) pero sigue siendo tuya.

## Fase de ejecución — comando `/goal`

Una vez creado y revisado el backlog en Jira:

```
/goal Trabaja los issues con label "spec-ready" del proyecto LDT
en orden de prioridad, uno a la vez (usa mcp-jira para leer/actualizar
issues y su integración con GitHub para ramas y PRs). Para issues de
frontend, usa design-notes.md como referencia visual obligatoria (colores,
tipografía, layout de cada componente) — no improvises estilos que no
estén ahí. Para cada issue: crea una rama, implementa el cambio en
backend/ o frontend/ según corresponda, corre el comando de verificación
indicado en la descripción del issue, y solo si pasa: haz commit, abre
un PR contra main, y mueve el issue a "Done" en Jira con un comentario
del PR. Si el check falla, itera sobre el mismo issue sin pasar al
siguiente. No mergees ningún PR tú mismo. Detente cuando no queden
issues con spec-ready abiertos, o tras 40 turnos.
```

**Nota:** el merge de PRs se deja manual a propósito en esta primera prueba,
para mantener control sobre lo que llega a `main` mientras se evalúa el
paradigma.