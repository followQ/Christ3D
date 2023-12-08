﻿using System;
using Christ3D.Domain.Core.Events;

namespace Christ3D.Domain.Events
{
    public class StudentUpdatedEvent : Event
    {
        public StudentUpdatedEvent(Guid id, string name, string email, DateTime birthDate,string phone)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
            Phone = phone;
            AggregateId = id;
        }
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public DateTime BirthDate { get; private set; }

        public string Phone { get; private set; }
    }
}