Talabat.APIs—a backend API project developed using the Onion Design Pattern to ensure a clean and scalable architecture. 🧅💻

🔧 Technologies and Packages Used:

AutoMapper.Extensions.Microsoft.DependencyInjection: To simplify object-to-object mapping between entities and DTOs.
Microsoft.EntityFrameworkCore.Design: For managing database design and migrations using Entity Framework Core.
Swashbuckle.AspNetCore: To generate Swagger documentation for the APIs.
Microsoft.AspNetCore.Identity.EntityFrameworkCore: For managing user identities and roles.
StackExchange.Redis: To efficiently store customer baskets.
Stripe.net: For integrating Stripe payments.
Microsoft.EntityFrameworkCore.SqlServer: To work with a SQL Server database.
Microsoft.AspNetCore.Authentication.JwtBearer: For JWT-based authentication.
📌 Overview of Endpoints in the Project:

🔹 Account Controller:

Register a new user: [HttpPost("Register")]
User login: [HttpPost("Login")]
Get current user: [HttpGet("GetCurrentUser")]
Get user address: [HttpGet("Address")]
Update user address: [HttpPut("Address")]
Check if email exists: [HttpGet("emailExists")]
🔹 Basket Controller:

Get a basket: [HttpGet("{BasketId}")]
Create a new basket: [HttpPost]
Delete a basket: [HttpDelete]
🔹 Orders Controller:

Create a new order: [HttpPost]
Get all orders: [HttpGet]
Get a specific order: [HttpGet("{id}")]
Delivery methods: [HttpGet("DeliveryMethods")]
🔹 Payment Controller:

Process payment for a specific basket: [HttpPost("{BasketId}")]
Receive webhook from Stripe: [HttpPost("webhook")]
🔹 Products Controller:

Get all products: [HttpGet]
Get a specific product: [HttpGet("{id}")]
Get product types: [HttpGet("Types")]
Get product brands: [HttpGet("Brands")]

Documentation : https://documenter.getpostman.com/view/29987270/2sAXjSzUfH#d363723b-eca3-404a-b05c-4c9dfcb21001
Git Repo : https://github.com/Ali-AbdElahady/Talabat.APIs.git
