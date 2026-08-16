# ApiPeliculas

API REST desarrollada con **ASP.NET Core Web API** para la gestión de películas y categorías.

El proyecto implementa autenticación mediante **JWT**, persistencia de datos con **Entity Framework Core y SQL Server**, documentación mediante **Swagger/OpenAPI** y separación de responsabilidades mediante repositorios y DTOs.

Fue desarrollado como parte de mi formación práctica en desarrollo backend con **C#/.NET**, con el objetivo de reforzar fundamentos de diseño y desarrollo de APIs REST.

## Tecnologías

* C#
* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Bearer Authentication
* AutoMapper
* Swagger/OpenAPI
* API Versioning

## API REST

La API proporciona operaciones para la gestión de películas y categorías, incluyendo:

* Consulta de películas.
* Consulta de una película específica.
* Creación de películas.
* Actualización de películas.
* Eliminación de películas.
* Gestión de categorías.
* Autenticación de usuarios.

La documentación completa de los endpoints puede consultarse mediante Swagger.

## Entity Framework Core

La persistencia de datos se implementa mediante **Entity Framework Core** utilizando un enfoque **Code First** y **SQL Server**.

El proyecto incluye migrations para crear y actualizar el esquema de la base de datos a partir de los modelos definidos en la aplicación.

## Autenticación con JWT

La API utiliza **JWT Bearer Authentication** para proteger los endpoints que requieren autenticación.

Swagger está configurado para permitir introducir el token JWT y probar los endpoints protegidos directamente desde la interfaz de documentación.

> Las claves utilizadas para JWT y cualquier otro dato sensible no deben almacenarse directamente en el repositorio.

## Versionamiento de API

El proyecto utiliza **API Versioning** para trabajar con diferentes versiones de la API.

En este proyecto, el versionamiento se implementó principalmente como parte del proceso de aprendizaje y **no representa una evolución real de la API**.

## Manejo de respuestas

Las respuestas de la API utilizan una estructura común para mantener un formato consistente entre diferentes endpoints.

Esto facilita que los consumidores de la API reciban una estructura de respuesta predecible.

## CORS

La aplicación incluye configuración de **Cross-Origin Resource Sharing (CORS)** para controlar los orígenes que pueden realizar solicitudes hacia la API.

La configuración debe adaptarse al entorno en el que se ejecute la aplicación.

## Swagger / OpenAPI

La API está documentada mediante **Swagger/OpenAPI**.

Swagger permite:

* consultar los endpoints disponibles;
* revisar parámetros y respuestas;
* probar las operaciones;
* autenticarse mediante Bearer Token;
* explorar las diferentes versiones de la API.

## Alcance del proyecto

Este proyecto representa **experiencia práctica de aprendizaje** en el desarrollo de APIs backend con C# y ASP.NET Core.

Su objetivo es servir como evidencia de mi formación práctica y del reforzamiento de conocimientos en:

* desarrollo de APIs REST;
* ASP.NET Core Web API;
* Entity Framework Core;
* SQL Server;
* DTOs y AutoMapper;
* Repository Pattern;
* JWT e Identity;
* CORS;
* API Versioning;
* Swagger/OpenAPI;
* Dependency Injection.

No pretende representar experiencia profesional ni el desarrollo de un sistema empresarial de gran escala.

## 

**Adrián Manuel Meneses López**

Ingeniero en Sistemas Computacionales enfocado en desarrollo backend con **C#/.NET, APIs REST y SQL Server**.
