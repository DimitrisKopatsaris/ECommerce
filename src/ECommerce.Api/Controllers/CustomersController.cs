using ECommerce.Domain.Entities;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers;

[ApiController] //enables automatic validation, model binding, and consistent HTTP 400 responses, when input is invalid
[Route("api/[controller]")] //auto resolves to api/customers
public class CustomersController : ControllerBase //it returns raw data (ActionResult<T>)
{
    private readonly ECommerceDbContext _db; //DI of my ECommerceDbContext
    public CustomersController(ECommerceDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        => Ok(await _db.Customers.AsNoTracking().ToListAsync()); //take all the customers from the DataBase (DbSet<Costumer>), for faster and less memory dont track changes, executes the SQL query asynchronously, return HTTP 200 with the JSON list.

    public record CreateCustomerRequest(string Name, string Email); // defines a small immutable data type for POST input.
    //record is a lightweight, immutable data container
    [HttpPost]
    public async Task<ActionResult<Customer>> Create(CreateCustomerRequest req)
    {
        var customer = new Customer { Name = req.Name, Email = req.Email }; //take the values from the req object and use them to make a new Customer entity (the one that corresponds to your database table). Basically mapping the incoming request model to the database entity model.
        _db.Customers.Add(customer); //mark the new costumer as one to be added when i call SaveChangesAsync().
        await _db.SaveChangesAsync(); //execute SQL insert command to add the new row in my customers table. After saving the EF Core automatically fills the customer.Id with the new database generated ID
        return CreatedAtAction(nameof(GetAll), new { id = customer.Id }, customer); //CreatedAtAction is a helper method provided by ControllerBase, sets the HTTP status code to 201 Created etc
    }
}
