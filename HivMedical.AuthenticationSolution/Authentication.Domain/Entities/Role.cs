﻿// using SharedKernel.Entities; // Temporarily commented out for Docker build
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
    }
}
