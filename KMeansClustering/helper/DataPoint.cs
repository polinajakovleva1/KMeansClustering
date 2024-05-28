using System;

namespace KMeansClustering.helper
{
    public class DataPoint
    {
        public string NameRegion { get; set;}
        public double RevenueFirstQuarter { get; set; }
        public double RevenueSecondQuarter { get; set; }
        public double RevenueThirdQuarter { get; set; }
        public double RevenueFourthQuarter { get; set; }
        public double RevenueYear { get; set; }
        public double AverageSalary { get; set; }
        public double Investment { get; set; }
        public double Employees { get; set; }
        public double WorkForce { get; set; }
        public double Enterprises { get; set; }
        public double Unemployment { get; set;}
        public double PovertyLevel { get; set;}
        public double Morbidity { get; set; }
        public double HospitalBeds { get; set; }
        public double HospitalCapacity { get; set; }
        public double HospitalStaff { get; set; }
        public double LifeExpectancy { get; set; }
        public double Arrivals { get; set; }
        public double Departed { get; set; }
        public double MigrationGrowth { get; set; }
        public int Cluster { get; set; }

        public DataPoint(string nameregion, double revenuefirstquarter, double revenuesecondquarter, double revenuethirdquarter, double revenuefourthquarter, double revenueyear, double averagesalary,
            double investment, double employees, double workforce, double enterprises, double unemployment, double povertylevel, double morbidity, double hospitalbeds, double hospitalcapacity, 
            double hospitalstaff, double lifeexpectancy, double arrivals, double departed, double migrationgrowth)
        {
            NameRegion = nameregion;
            RevenueFirstQuarter = revenuefirstquarter;
            RevenueSecondQuarter = revenuesecondquarter;
            RevenueThirdQuarter = revenuethirdquarter;
            RevenueFourthQuarter = revenuefourthquarter;
            RevenueYear = revenueyear;
            AverageSalary = averagesalary;
            Investment = investment;
            Employees = employees;
            WorkForce = workforce;
            Enterprises = enterprises;
            Unemployment = unemployment;
            PovertyLevel = povertylevel;
            Morbidity = morbidity;
            HospitalBeds = hospitalbeds;
            HospitalCapacity = hospitalcapacity;
            HospitalStaff = hospitalstaff;
            LifeExpectancy = lifeexpectancy;
            Arrivals = arrivals;
            Departed = departed;
            MigrationGrowth = migrationgrowth;
            Cluster = 0;
        }
        public DataPoint(DataPoint f)
        {
            NameRegion = f.NameRegion;
            RevenueFirstQuarter = f.RevenueFirstQuarter;
            RevenueSecondQuarter = f.RevenueSecondQuarter;
            RevenueThirdQuarter = f.RevenueThirdQuarter;
            RevenueFourthQuarter = f.RevenueFourthQuarter;
            RevenueYear = f.RevenueYear;
            AverageSalary = f.AverageSalary;
            Investment = f.Investment;
            Employees = f.Employees;
            WorkForce = f.WorkForce;
            Enterprises = f.Enterprises;
            Unemployment = f.Unemployment;
            PovertyLevel = f.PovertyLevel;
            Morbidity = f.Morbidity;
            HospitalBeds = f.HospitalBeds;
            HospitalCapacity = f.HospitalCapacity;
            HospitalStaff = f.HospitalStaff;
            LifeExpectancy = f.LifeExpectancy;
            Arrivals = f.Arrivals;
            Departed = f.Departed;
            MigrationGrowth = f.MigrationGrowth;
            Cluster = 0;
        }
        public DataPoint()
        {

        }
    }
}
