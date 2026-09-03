# Notas de diseño — Tareas de la semana (iteración 2)

Extraído de la maqueta compartida por el usuario (`Tareas_de_la_semana.html`).
Este documento es la referencia visual que debe usar el loop de desarrollo
para las historias de frontend — el archivo original de la maqueta usa un
formato de plantilla propio (no HTML/CSS estándar), así que no se incluye
tal cual en el repo; estos son los valores concretos extraídos de él.

## Paleta

| Uso | Color |
|---|---|
| Fondo de página | `#f3f6f3` |
| Fondo de tarjetas/paneles | `#fff` |
| Fondo de franjas (header interno, footer) | `#fafcfa` |
| Borde estándar | `#e3eae5` |
| Borde de separadores internos | `#eaf0eb` / `#eff4f0` |
| Texto principal | `#1c2622` |
| Texto secundario | `#495650` |
| Texto terciario / metadatos | `#7d8a84` |
| Acento primario (botones, links, activo) | `#2f9575` (hover `#237558`) |
| Fondo de badge "semana actual" | `#e4f2ea` (texto `#2f9575`) |
| Peligro / eliminar | `#b4453c` (hover `#94352d`) |
| Fondo hover de fila | `#f8fbf8` |

**Prioridad (color del punto + texto):**
- Alta: `#b4453c`
- Media: `#b5852f`
- Baja: `#7d9a8f`

## Tipografía

- Familia principal: `IBM Plex Sans` (fallback `system-ui, sans-serif`)
- Familia monoespaciada (metadatos, fechas, contadores, footer): `IBM Plex Mono`
- Título (`h1`, "Tareas de la semana"): 29px, weight 600, letter-spacing -0.02em
- Eyebrow superior ("Planeación semanal · ..."): 11px, IBM Plex Mono, uppercase, letter-spacing .14em, color `#7d8a84`
- Texto de tarea: 15px
- Metadatos (rango de semana, contador, footer): 11.5–12px, IBM Plex Mono, color `#7d8a84`/`#95a19b`

## Layout general

- Contenedor centrado, `max-width: 920px`, padding `44px 32px 80px`
- Tarjeta principal: `border-radius: 20px`, borde `#e3eae5`, sombra sutil
- Radios de botones/inputs: 10-11px; radios de tarjetas/modales: 20px

## Componentes

**Header de la app:** eyebrow + título a la izquierda, botón primario
"+ Nueva tarea" a la derecha (verde `#2f9575`, `border-radius: 11px`).

**Barra de navegación de semana** (dentro de la tarjeta, fondo `#fafcfa`):
botones `‹`/`›` circulares-cuadrados de 34x34px, etiqueta de semana
("Semana N") + badge "Semana actual" cuando aplica, rango de fechas debajo
en mono, botón "Ir a hoy" a la derecha.

**Barra de progreso y filtros:** contador "`{done}/{total}` completadas" +
barra de progreso delgada (6px, verde sobre fondo `#eaf0eb`). A la derecha,
3 filtros tipo pill agrupados en un contenedor `#eff4f0`: el filtro activo
tiene fondo blanco y texto oscuro con sombra sutil; los inactivos son
transparentes con texto `#77857e`.

**Fila de tarea:** checkbox cuadrado redondeado (20x20px) a la izquierda,
descripción + punto de color con label de prioridad debajo, botón de
eliminar (ícono de bote de basura) a la derecha que solo se resalta en
rojo al hover. Tareas completadas: texto tachado y color atenuado.

**Estado vacío:** cuadro punteado 44x44px centrado + mensaje principal +
mensaje secundario (ej. "Cambia de semana o ajusta el filtro para ver el
resto de las tareas").

**Footer de la tarjeta:** nota pequeña a la izquierda + botón "Pasar
pendientes a la próxima semana" a la derecha (outline, se pone verde al hover).

**Modal "Nueva tarea":** overlay oscuro con blur, tarjeta blanca centrada
(max-width 470px, `border-radius: 20px`). Título + subtítulo indicando a
qué semana se agrega. Campo de descripción + selector de prioridad (3
botones tipo pill con punto de color). Footer con "Cancelar" (outline) y
"Agregar tarea" (verde, disabled si la descripción está vacía).

**Modal de confirmar eliminar:** overlay igual, tarjeta más pequeña
(max-width 400px), mensaje con el título de la tarea en negritas, botones
"Cancelar" (outline) y "Eliminar" (rojo `#b4453c`, hover `#94352d`).

## Notas de implementación

- Los filtros son: **Todas**, **Pendientes**, **Completadas** (ese orden).
- El botón "+" del header y el botón "Nueva tarea" abren el mismo modal.
- Las animaciones de entrada de modal (fade + pop sutil) son un detalle
  agradable pero no bloqueante — no es criterio de aceptación de ningún
  issue, es "nice to have" si el tiempo lo permite.