﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACMEdraw.Data;
using ACMEdraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ACMEdraw.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrawsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DrawsController> _logger;
        public DrawsController(ApplicationDbContext context, ILogger<DrawsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // GET: api/<DrawsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Draw>>> GetDraws()
        {
            return await _context.Draws.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Draw>> GetDrawById(int id)
        {
            var draw = await _context.Draws.FindAsync(id);
            if (draw == null)
            {
                return NotFound();
            }
            return draw;
        }


        // POST api/<DrawsController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DrawContract value)
        {
            try
            {
                var newDraw = new Draw { Email = value.Email };
                Guid serialNumber = new Guid(value.serialNumber);

                var person = await _context.People
                    .FirstOrDefaultAsync(p => 
                        p.FirstName == value.firstName && p.LastName == value.lastName);

                var newPerson = new Person {LastName = value.lastName,
                    FirstName = value.firstName,BirthDate = DateTime.Parse(value.birthDate) };

                newDraw.Person =person!=null?person: newPerson;
                newDraw.Product = await _context.Products
                    .FirstAsync(p=>
                        p.SerialNumber.Equals(serialNumber));
                
                if (person==null)
                {
                    await _context.AddAsync(newPerson);
                }
                
                await _context.AddAsync(newDraw);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
                
            }
           
        }
        //TODO implement put
        // DELETE api/<DrawsController>/5
        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            var draw = await _context.Draws.FindAsync(id);
            _context.Draws.Remove(draw);
            await _context.SaveChangesAsync();
        }
    }
}
