﻿using Microsoft.EntityFrameworkCore;

namespace MindvizServer.Core.Models.SubModels
{
    [Owned]
    public class Name
    {
        public string First { get; set; }
        public string? Middle { get; set; }
        public string Last { get; set; }
    }
}
