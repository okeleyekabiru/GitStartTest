﻿namespace Authentication.API.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }  
        public string? Name { get; set; } 
        public string? Description { get; set; }  
    }
}
