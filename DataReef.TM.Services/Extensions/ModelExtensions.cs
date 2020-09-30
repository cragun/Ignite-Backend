using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.Geo.DataViews;
using DataReef.Integrations.Microsoft.PowerBI.Models;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DataViews.SelfTrackedKPIs;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.DTOs.Reports;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ModelExtensions
{
    public static PBI_Consumption ToPBI(this Property property, Guid ouID)
    {
        if (property?.PowerConsumptions == null)
        {
            return null;
        }

        return property.PowerConsumptions.ToPBI(property.Guid, ouID);
    }

    public static PBI_Consumption ToPBI(this ICollection<PropertyPowerConsumption> consumption, Guid propertyId, Guid ouID)
    {
        return consumption.Cast<PowerConsumption>().ToList().ToPBI(propertyId, ouID);
    }

    public static PBI_Consumption ToPBI(this ICollection<SolarSystemPowerConsumption> consumption, Guid propertyId, Guid ouID)
    {
        return consumption.Cast<PowerConsumption>().ToList().ToPBI(propertyId, ouID);
    }

    public static PBI_Consumption ToPBI(this List<PowerConsumption> consumption, Guid propertyId, Guid ouID)
    {
        if ((consumption?.Count ?? 0) == 0)
        {
            return new PBI_Consumption
            {
                Source = SmartPrincipal.DeviceType.ToString(),
                PropertyID = propertyId,
                OUID = ouID,
            };
        }

        return new PBI_Consumption
        {
            Source = SmartPrincipal.DeviceType.ToString(),
            PropertyID = propertyId,
            OUID = ouID,
            TotalWatts = (double)consumption.Sum(pc => pc.Watts),
            TotalCost = (double)consumption.Sum(pc => pc.Cost),
            AverageWatts = (double)(consumption.Count > 0 ? consumption.Average(pc => pc.Watts) : 0),
            AverageCost = (double)(consumption.Count > 0 ? consumption.Average(pc => pc.Cost) : 0),
            TotalConsumptionMonths = consumption.Count,
            ManuallyEnteredConsumptionMonths = consumption.Count(pc => pc.IsManuallyEntered)
        };
    }

    public static PBI_RoofDrawing ToPBI(this List<RoofPlane> roofPlanes, Guid propertyId, Guid ouID, bool isNew)
    {
        if (roofPlanes == null || roofPlanes.Count == 0)
        {
            return null;
        }

        return new PBI_RoofDrawing
        {
            SalesRepID = SmartPrincipal.UserId,
            OUID = ouID,
            PropertyID = propertyId,
            DeviceType = SmartPrincipal.DeviceType.ToString(),
            RoofPlanesCount = roofPlanes.Count,
            Panels = string.Join(" | ", roofPlanes
                                        .Where(rp => rp.PanelsCount > 0
                                                  && rp.SolarPanel != null)
                                        .GroupBy(rp => rp.SolarPanel)
                                        .Select(rp => $"{rp.Key.Name}[{rp.Key.Watts}W] ({rp.Sum(r => r.PanelsCount)})")),
            PanelsCount = roofPlanes.Sum(rp => rp.PanelsCount),
            Action = isNew ? "Create" : "Update"
        };
    }

    public static Shape ToGeoShape(this BaseShape shape)
    {
        var centroidAndRadius = shape.WellKnownText.GetCentroidAndRadius();

        return new Shape
        {
            ShapeID = shape.Guid.ToString().ToLower(),
            ShapeTypeID = shape.ShapeTypeID,
            ShapeReduced = shape.WellKnownText,
            ShapeName = shape.Name,
            CentroidLon = (float)centroidAndRadius.Item1,
            CentroidLat = (float)centroidAndRadius.Item2,
            Radius = (float)centroidAndRadius.Item3,
        };
    }

    internal static SelfTrackedStatistics ToStatisticRow(this IGrouping<string, SimplePersonKPI> rows, StatisticDates dates)
    {
        return new SelfTrackedStatistics
        {
            KPIName = rows.Key,
            Actions = new SelfTrackedKPIByDate
            {
                AllTime = rows.Sum(c => c.Value),
                ThisYear = rows
                        .Where(c => c.DateCreated >= dates.YearStart)
                        .Sum(c => c.Value),
                ThisMonth = rows
                        .Where(c => c.DateCreated >= dates.MonthStart)
                        .Sum(c => c.Value),
                ThisWeek = rows
                        .Where(c => c.DateCreated >= dates.CurrentWeekStart)
                        .Sum(c => c.Value),
                Today = rows
                        .Where(c => c.DateCreated >= dates.TodayStart)
                        .Sum(c => c.Value),
                SpecifiedDay = dates.HasSpecifiedDay ? rows
                                                    .Where(c => c.DateCreated >= dates.SpecifiedStart.Value
                                                             && c.DateCreated < dates.SpecifiedEnd.Value)
                                                    .Sum(c => c.Value)
                                                : 0
            }
        };
    }

    internal static List<SelfTrackedStatistics> ToStatisticRows(this IGrouping<Guid, PersonKPI> rows, StatisticDates dates)
    {
        return rows.Select(p => new SimplePersonKPI(p))
                        .GroupBy(p => p.Name)
                        .Select(g => g.ToStatisticRow(dates))
                        .ToList();
    }

    internal static int GetStatisticForReportingPeriod(this SelfTrackedStatistics stat, ReportingPeriod reportingPeriod)
    {
        switch (reportingPeriod)
        {
            case ReportingPeriod.AllTime:
                return stat.Actions.AllTime;
            case ReportingPeriod.ThisYear:
                return stat.Actions.ThisYear;
            case ReportingPeriod.ThisMonth:
                return stat.Actions.ThisMonth;
            case ReportingPeriod.ThisWeek:
                return stat.Actions.ThisWeek;
            case ReportingPeriod.Today:
                return stat.Actions.Today;
            case ReportingPeriod.SpecifiedDay:
                return stat.Actions.SpecifiedDay;
        }

        return 0;
    }

    internal static long GetStatisticForInquiryReportingPeriod(this InquiryStatisticsForOrganization stat, ReportingPeriod reportingPeriod)
    {
        switch (reportingPeriod)
        {
            case ReportingPeriod.AllTime:
                return stat.Actions.AllTime;
            case ReportingPeriod.ThisYear:
                return stat.Actions.ThisYear;
            case ReportingPeriod.ThisMonth:
                return stat.Actions.ThisMonth;
            case ReportingPeriod.ThisWeek:
                return stat.Actions.ThisWeek;
            case ReportingPeriod.Today:
                return stat.Actions.Today;
            case ReportingPeriod.SpecifiedDay:
                return stat.Actions.SpecifiedDay;
        }

        return 0;
    }

    internal static List<T> Find<T>(this List<KeyValuePair<Guid, List<T>>> data, Guid key) where T : class
    {
        if (data?.Any(d => d.Key == key) != true)
        {
            return null;
        }

        return data
                .FirstOrDefault(d => d.Key == key)
                .Value;
    }

    internal static ActiveUserDTO HighestRole(this IGrouping<Guid, ActiveUserDTO> data)
    {
        return data.OrderBy(d => d.RoleIndex).FirstOrDefault();
    }
}
