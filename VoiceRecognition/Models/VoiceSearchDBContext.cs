using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace VoiceRecognition.Models
{
    public class VoiceSearchDBContext:DbContext
    {
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Airline> Airlines { get; set; }
    }
}