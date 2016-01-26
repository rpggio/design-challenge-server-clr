# Design Challenge Server

A server for running code challenges in private Git repositories. The system uses GitBlit to generate and manage repositories for users.
When a user checks in a solution, the server pull latest and runs acceptance tests against their code.
If the solution passes, the system will promote the user to the next stage, pushing into their repository the new acceptance tests.

This repository contains the back-end processors and services for running on a Windows server. A service bus is used internally to coordinate system operations.
Requires Gitblit, RabbitMQ, MSBuild tools for building solutions, Ruby and Rake for running acceptance tests.
Deployment and configuration tools are for Amazon EC2.
Currently, the system only supports solutions in C#, but it is architected to support any language which can imlement a minimal HTTP service (so, any language, technically).

## GateScheduler Challenge

 GateScheduler is the first challenge designed for the system. The readme for the challenge describes how it works:

> For this challenge, you will be building an application to schedule gates at an airport. The application will receive information about the inbound and outbound flights at the airport. It will be responsible for scheduling the flights at available gates according to the constraints given. 

> The application will be implemented as a simple in-process HTTP server. All interaction with the application will be through the web API. The server will be minimally simple, providing only basic request/response behavior with one request at a time. For most languages, there are capable, lightweight in-process libraries available that will provide all the needed HTTP functionality.

> The requirements for the application will be provided in stages. After a stage is completed, satisfying the provided acceptance tests, the requirements for the next stage are provided. The requirements are provided as tests written using Cucumber, a library for Ruby that provides a language for writing feature descriptions that are executable.
