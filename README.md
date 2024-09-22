# Gitstart Microservices 

This repository contains three interconnected microservices for user authentication, product management, and inventory management. All services are built using ASP.NET Core and use Docker Compose to simplify the setup process.

## Overview

### Microservices

1. **Authentication Service**
   - User registration, login, and management functionalities.

2. **Product Service**
   - Manage products, including CRUD operations and pagination.

3. **Inventory Service**
   - Handle inventory items and stock transactions.

### Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- Redis
- Docker & Docker Compose

## Prerequisites

- Docker
- Docker Compose

## Getting Started

### Clone the Repository

```bash
git clone <repository-url>
cd <repository-directory>
```

### Docker Compose Setup

This project uses Docker Compose to set up the environment. The `docker-compose.yml` file is configured to run all three microservices alongside SQL Server and Redis.

### Configuration

Ensure that your `appsettings.json` or environment variables are set correctly for database connections and Redis configuration in each service.

### Running the Application

To build and start the application with all dependencies, run:

```bash
docker-compose up --build
```

This command will:

1. Build the microservice images.
2. Start SQL Server and Redis containers.
3. Spin up the Authentication, Product, and Inventory APIs.

### Accessing the APIs

Once the containers are up, the APIs will be accessible at:

- **Authentication API**: `http://localhost:<auth-api-port>/api/auth`
- **Product API**: `http://localhost:<product-api-port>/api/products`
- **Inventory API**: `http://localhost:<inventory-api-port>/api/inventory`

Replace `<auth-api-port>`, `<product-api-port>`, and `<inventory-api-port>` with the ports configured in the `docker-compose.yml` file.

### API Endpoints

#### Authentication Service

| Method | Endpoint                   | Description                     |
|--------|----------------------------|---------------------------------|
| POST   | /api/auth/register         | Register a new user            |
| POST   | /api/auth/login            | Authenticate a user             |
| PUT    | /api/auth/user/{userId}   | Update user details             |
| DELETE | /api/auth/user/{userId}   | Delete a user                   |

#### Product Service

| Method | Endpoint                   | Description                     |
|--------|----------------------------|---------------------------------|
| GET    | /api/products              | Retrieve all products           |
| GET    | /api/products/{id}         | Get product by ID               |
| POST   | /api/products              | Create a new product            |
| PUT    | /api/products/{id}         | Update an existing product      |
| DELETE | /api/products/{id}         | Delete a product                |

#### Inventory Service

| Method | Endpoint                   | Description                     |
|--------|----------------------------|---------------------------------|
| GET    | /api/inventory             | Retrieve all inventory items    |
| GET    | /api/inventory/{id}        | Get inventory item by ID        |
| POST   | /api/inventory             | Create a new inventory item     |
| PUT    | /api/inventory/{id}        | Update an existing inventory item|
| DELETE | /api/inventory/{id}        | Delete an inventory item        |

## Running Tests

To run unit and integration tests, you can use the following command:

```bash
dotnet test
```

## Stopping the Application

To stop and remove the containers, run:

```bash
docker-compose down
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue.

## License