using System;
using System.Collections.Generic;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Practitioner.Domain.Enums;

namespace Misan.Modules.Practitioner.Domain.Entities;

public sealed class Clinic : Entity
{
    private readonly List<ClinicService> _services = new();
    private readonly List<ClinicFee> _fees = new();
    private readonly List<ClinicSchedule> _schedules = new();

    private Clinic(
        Guid id,
        Guid hakeemId,
        string name,
        string address,
        string city,
        string phone,
        double mapLat,
        double mapLng)
        : base(id)
    {
        HakeemId = hakeemId;
        Name = name;
        Address = address;
        City = city;
        Phone = phone;
        MapLat = mapLat;
        MapLng = mapLng;
        IsActive = true;
    }

    private Clinic() { }

    public Guid HakeemId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public double MapLat { get; private set; }
    public double MapLng { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<ClinicService> Services => _services.AsReadOnly();
    public IReadOnlyCollection<ClinicFee> Fees => _fees.AsReadOnly();
    public IReadOnlyCollection<ClinicSchedule> Schedules => _schedules.AsReadOnly();

    public static Clinic Create(Guid hakeemId, string name, string address, string city, string phone, double lat, double lng)
    {
        return new Clinic(Guid.NewGuid(), hakeemId, name, address, city, phone, lat, lng);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void AddService(string name)
    {
        _services.Add(new ClinicService(Guid.NewGuid(), Id, name));
    }

    public void AddFee(ClinicFeeType type, decimal amount, string currency = "PKR")
    {
        _fees.Add(new ClinicFee(Guid.NewGuid(), Id, type, amount, currency));
    }

    public void AddSchedule(DayOfWeek day, TimeSpan morningStart, TimeSpan morningEnd, TimeSpan afternoonStart, TimeSpan afternoonEnd, bool isClosed)
    {
        // Simple add, real logic might replace existing day
        _schedules.Add(new ClinicSchedule(Guid.NewGuid(), Id, day, morningStart, morningEnd, afternoonStart, afternoonEnd, isClosed));
    }
}
