using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VoiceRecognition.Models
{
    public class Flight
    {
        public long Id { get; set; }

        public string Airline { get; set; }

        public double Price { get; set; }

        public string Time { get; set; }

        public string Depurture { get; set; }
        public string Destination { get; set; }

        public Flight(long id, string airline, double price, string time, string dep, string dest)
        {
            Id = id;
            Airline = airline;
            Price = price;
            Time = time;
            Depurture = dep;
            Destination = dest;
        }

    }
}