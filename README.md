# TODO App semanal

Aplicación simple de lista de tareas ("compromisos de la semana"). Ver
[Especificación](todo-app-especificacion.md) para el detalle completo.

## Estructura

```
todo/
├── backend/    (ASP.NET Core Web API, en memoria)
├── frontend/   (Angular)
└── README.md
```

## Backend

```
cd backend
dotnet build
dotnet test
dotnet run
```

## Frontend

```
cd frontend
npm install
ng build
ng test --watch=false
ng serve
```
