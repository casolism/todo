# Especificación — TODO App semanal

## Objetivo

Aplicación simple de lista de tareas ("compromisos de la semana"): descripciones
breves con checkbox que marca pendiente/terminada. Sin autenticación, sin
persistencia avanzada (memoria es suficiente para esta primera versión).

Sirve como proyecto de prueba para el paradigma de "looping" (`/goal`) en
Claude Code, con Jira + GitHub como backlog y repositorio, vía MCP.

## Alcance

**Incluye:**
- Ver lista de tareas de la semana
- Agregar una tarea nueva (descripción breve)
- Marcar/desmarcar una tarea como terminada (checkbox)
- Borrar una tarea

**No incluye (fuera de alcance para esta versión):**
- Autenticación / usuarios
- Persistencia en base de datos (queda en memoria del backend)
- Edición de tareas (cambiar la descripción de una ya creada)
- Fechas límite, prioridades, categorías

## Estructura del repositorio (monorepo)

```
todo-app/
├── backend/        (ASP.NET Core Web API)
├── frontend/        (Angular)
└── README.md
```

## Stack

- **Backend:** ASP.NET Core Web API (.NET), almacenamiento en memoria
- **Frontend:** Angular

## Modelo de datos

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
/goal Afina la descripción de cada issue del proyecto [LDT] que no
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
/goal Trabaja los issues con label "spec-ready" del proyecto [LDT]
en orden de prioridad, uno a la vez (usa mcp-jira para leer/actualizar
issues y su integración con GitHub para ramas y PRs). Para cada issue:
crea una rama, implementa el cambio en backend/ o frontend/ según
corresponda, corre el comando de verificación indicado en la descripción
del issue, y solo si pasa: haz commit, abre un PR contra main, y mueve
el issue a "Done" en Jira con un comentario del PR. Si el check falla,
itera sobre el mismo issue sin pasar al siguiente. No mergees ningún PR
tú mismo. Detente cuando no queden issues con spec-ready abiertos, o
tras 40 turnos.
```

**Nota:** el merge de PRs se deja manual a propósito en esta primera prueba,
para mantener control sobre lo que llega a `main` mientras se evalúa el
paradigma.