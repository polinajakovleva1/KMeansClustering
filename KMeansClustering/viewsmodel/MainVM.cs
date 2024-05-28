using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using KMeansClustering.helper;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using System.Collections;

namespace KMeansClustering.viewsmodel
{
    internal class MainVM : EventWindow
    {
        private bool _isDownloadFile;
        public bool IsDownloadFile
        {
            get { return _isDownloadFile; }
            set
            {
                if (_isDownloadFile != value)
                {
                    _isDownloadFile = value;
                    OnPropertyChanged(nameof(IsDownloadFile));
                }
            }
        }

        private bool _isClustering;
        public bool IsClustering
        {
            get { return _isClustering; }
            set
            {
                if (_isClustering != value)
                {
                    _isClustering = value;
                    OnPropertyChanged(nameof(IsClustering));
                }
            }
        }

        private bool _isUploadFile;
        public bool IsUploadFile
        {
            get { return _isUploadFile; }
            set
            {
                if (_isUploadFile != value)
                {
                    _isUploadFile = value;
                    OnPropertyChanged(nameof(IsUploadFile));
                }
            }
        }

        private bool _isExit;
        public bool IsExit
        {
            get { return _isExit; }
            set
            {
                if (_isExit != value)
                {
                    _isExit = value;
                    OnPropertyChanged(nameof(IsExit));
                }
            }
        }

        public MainVM()
        {
            IsDownloadFile = true;
            IsClustering = false;
            IsUploadFile = false;
            IsExit = true;
        }

        private List<DataPoint> userData = new List<DataPoint>();
        private List<DataPoint> normalizeData = new List<DataPoint>();
        private List<DataPoint> clustersData = new List<DataPoint>();
        private string inputFileName;
        private int clusterCount = 5;

        private RelayCommand downloadfile;
        private RelayCommand uploadfile;
        private RelayCommand clustering;
        private RelayCommand exit;

        public RelayCommand DownloadFile
        {
            get
            {
                return downloadfile ??= new(obj =>
                {
                    var fileDialog = new OpenFileDialog { Filter = "Excel (*.xlsx)| *.xlsx" };
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            IsDownloadFile = false;
                            IsExit = false;
                            ReadData(fileDialog.FileName);
                            if(userData.Count > 0)
                            {
                                inputFileName = Path.GetFileNameWithoutExtension(fileDialog.FileName);
                                MessageBox.Show("Файл успешно прочитан");
                                IsClustering = true;
                                IsExit = true;
                            }
                            else
                            {
                                MessageBox.Show("В файле нет данных");
                                IsDownloadFile = true;
                                IsExit = true;
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Ошибка при чтении файла");
                            IsDownloadFile = true;
                            IsExit = true;
                        }
                    }
                });
            }

        }

        public RelayCommand Clustering =>
            clustering ??= new(obj =>
            {
                IsExit = false;
                IsClustering = false;
                for (int i = 0; i < clusterCount; i++)
                {
                    clustersData.Add(new DataPoint() { Cluster = i });
                }
                NormalizeData();
                Cluster(normalizeData);
                MessageBox.Show("Кластеризация данных выполнена");
                IsUploadFile = true;
                IsExit = true;
            });

        public RelayCommand UploadFile
        {
            get
            {
                return uploadfile ??= new(obj =>
                {
                    IsExit = false;
                    IsUploadFile = false;
                    var fileDialog = new OpenFileDialog { Filter = "Excel (*.xlsx)| *.xlsx" };
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            WriteData(fileDialog.FileName);
                            MessageBox.Show("Результат успешно записан");
                            Clear();
                            IsDownloadFile = true;
                            IsExit = true;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Ошибка при записи результатов");
                            IsDownloadFile = true;
                            IsExit = true;
                        }
                    }
                });
            }
        }

        public RelayCommand Exit
        {
            get
            {
                return exit ??= new(obj =>
                {

                    IsExit = false;
                    IsDownloadFile = false;
                    IsClustering = false;
                    IsUploadFile = false;
                    Clear();

                    OnRequestClose();
                });
            }
        }

        private void ReadData(string fileName)
        {
            var connectionString = string.Format(
                $"Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=\"EXCEL 12.0; HDR=Yes;\";Data Source={fileName}"
            );
            var excelData = new DataSet();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                var objDA = new OleDbDataAdapter("select * from [Лист1$A:U]", connection);
                objDA.Fill(excelData);
            }
            var table = excelData.Tables[0];
            var x = new List<DataPoint>();
            for (int j = 0; j < table.Rows.Count; j++)
            {
                var obs = new List<double>();
                string name = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i == 0) name = table.Rows[j][i].ToString();
                    else obs.Add((double)table.Rows[j][i]);
                }
                x.Add(new(name, obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10], obs[11], obs[12], obs[13], obs[14], obs[15], obs[16], obs[17], obs[18], obs[19]));

            }
            userData = new List<DataPoint>(x);
        }

        private void WriteData(string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == inputFileName);

                if (worksheet != null)
                {
                    excelPackage.Workbook.Worksheets.Delete(inputFileName);
                }

                worksheet = excelPackage.Workbook.Worksheets.Add(inputFileName);

                worksheet.Cells["A1"].Value = "Кластер";
                worksheet.Cells["B1"].Value = "Название региона";
                worksheet.Cells["C1"].Value = "Доходы за 1 квартал";
                worksheet.Cells["D1"].Value = "Доходы за 2 квартал";
                worksheet.Cells["E1"].Value = "Доходы за 3 квартал";
                worksheet.Cells["F1"].Value = "Доходы за 4 квартал";
                worksheet.Cells["G1"].Value = "Доходы за весь год";
                worksheet.Cells["H1"].Value = "Заработная плата";
                worksheet.Cells["I1"].Value = "Инвестиции на душу населения";
                worksheet.Cells["J1"].Value = "Численность занятых";
                worksheet.Cells["K1"].Value = "Численность рабочей силы";
                worksheet.Cells["L1"].Value = "Число предприятий";
                worksheet.Cells["M1"].Value = "Безработица";
                worksheet.Cells["N1"].Value = "Уровень бедности";
                worksheet.Cells["O1"].Value = "Заболеваемость на 1000 человек";
                worksheet.Cells["P1"].Value = "Количество коек";
                worksheet.Cells["Q1"].Value = "Мощность больниц";
                worksheet.Cells["R1"].Value = "Количество мед. персонала";
                worksheet.Cells["S1"].Value = "Продолжительность жизни";
                worksheet.Cells["T1"].Value = "Прибывшие";
                worksheet.Cells["U1"].Value = "Выбывшие";
                worksheet.Cells["V1"].Value = "Миграционный прирост";


                int rowNumber = 2;

                var x = userData.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
                foreach (var group in x)
                {
                    foreach (var item in group)
                    {
                        worksheet.Cells[rowNumber, 1].Value = item.Cluster;
                        worksheet.Cells[rowNumber, 2].Value = item.NameRegion;
                        worksheet.Cells[rowNumber, 3].Value = item.RevenueFirstQuarter;
                        worksheet.Cells[rowNumber, 4].Value = item.RevenueSecondQuarter;
                        worksheet.Cells[rowNumber, 5].Value = item.RevenueThirdQuarter;
                        worksheet.Cells[rowNumber, 6].Value = item.RevenueFourthQuarter;
                        worksheet.Cells[rowNumber, 7].Value = item.RevenueYear;
                        worksheet.Cells[rowNumber, 8].Value = item.AverageSalary;
                        worksheet.Cells[rowNumber, 9].Value = item.Investment;
                        worksheet.Cells[rowNumber, 10].Value = item.Employees;
                        worksheet.Cells[rowNumber, 11].Value = item.WorkForce;
                        worksheet.Cells[rowNumber, 12].Value = item.Enterprises;
                        worksheet.Cells[rowNumber, 13].Value = item.Unemployment;
                        worksheet.Cells[rowNumber, 14].Value = item.PovertyLevel;
                        worksheet.Cells[rowNumber, 15].Value = item.Morbidity;
                        worksheet.Cells[rowNumber, 16].Value = item.HospitalBeds;
                        worksheet.Cells[rowNumber, 17].Value = item.HospitalCapacity;
                        worksheet.Cells[rowNumber, 18].Value = item.HospitalStaff;
                        worksheet.Cells[rowNumber, 19].Value = item.LifeExpectancy;
                        worksheet.Cells[rowNumber, 20].Value = item.Arrivals;
                        worksheet.Cells[rowNumber, 21].Value = item.Departed;
                        worksheet.Cells[rowNumber, 22].Value = item.MigrationGrowth;
                        rowNumber++;
                    }
                    rowNumber += 7;
                }

                excelPackage.Save();
            }
        }

        public void Cluster(List<DataPoint> data)
        {
            bool changed = true;
            bool success = true;
            InitializeCentroids();

            int maxIteration = data.Count * 10;
            int threshold = 0;
            while (success == true && changed == true && threshold < maxIteration)
            {
                ++threshold;
                success = UpdateDataPointMeans();
                changed = UpdateClusterMembership();
            }
        }

        private void NormalizeData()
        {
            double revenuefirstquarter = 0.0;
            double revenuesecondquarter = 0.0;
            double revenuethirdquarter = 0.0;
            double revenuefourthquarter = 0.0;
            double revenueyear = 0.0;
            double averagesalary = 0.0;
            double investment = 0.0;
            double employees = 0.0;
            double workforce = 0.0;
            double enterprises = 0.0;
            double unemployment = 0.0;
            double povertylevel = 0.0;
            double morbidity = 0.0;
            double hospitalbeds = 0.0;
            double hospitalcapacity = 0.0;
            double hospitalstaff = 0.0;
            double lifeexpectancy = 0.0;
            foreach (DataPoint f in userData)
            {
                revenuefirstquarter += f.RevenueFirstQuarter;
                revenuesecondquarter += f.RevenueSecondQuarter;
                revenuethirdquarter += f.RevenueThirdQuarter;
                revenuefourthquarter += f.RevenueFourthQuarter;
                revenueyear += f.RevenueYear;
                averagesalary += f.AverageSalary;
                investment += f.Investment;
                employees += f.Employees;
                workforce += f.WorkForce;
                enterprises += f.Enterprises;
                unemployment += f.Unemployment;
                povertylevel += f.PovertyLevel;
                morbidity += f.Morbidity;
                hospitalbeds += f.HospitalBeds;
                hospitalcapacity += f.HospitalCapacity;
                hospitalstaff += f.HospitalStaff;
                lifeexpectancy += f.LifeExpectancy;
            }
            double revenuefirstquarterMean = revenuefirstquarter / userData.Count;
            double revenuesecondquarterMean = revenuesecondquarter / userData.Count;
            double revenuethirdquarterMean = revenuethirdquarter / userData.Count;
            double revenuefourthquarterMean = revenuefourthquarter / userData.Count;
            double revenueyearMean = revenueyear / userData.Count;
            double averagesalaryMean = averagesalary / userData.Count;
            double investmentMean = investment / userData.Count;
            double employeesMean = employees / userData.Count;
            double workforceMean = workforce / userData.Count;
            double enterprisesMean = enterprises / userData.Count;
            double unemploymentMean = unemployment / userData.Count;
            double povertylevelMean = povertylevel / userData.Count;
            double morbidityMean = morbidity / userData.Count;
            double hospitalbedsMean = hospitalbeds / userData.Count;
            double hospitalcapacityMean = hospitalcapacity / userData.Count;
            double hospitalstaffMean = hospitalstaff / userData.Count;
            double lifeexpectancyMean = lifeexpectancy / userData.Count;
            double revenuefirstquarterSum = 0.0;
            double revenuesecondquarterSum = 0.0;
            double revenuethirdquarterSum = 0.0;
            double revenuefourthquarterSum = 0.0;
            double revenueyearSum = 0.0;
            double averagesalarySum = 0.0;
            double investmentSum = 0.0;
            double employeesSum = 0.0;
            double workforceSum = 0.0;
            double enterprisesSum = 0.0;
            double unemploymentSum = 0.0;
            double povertylevelSum = 0.0;
            double morbiditySum = 0.0;
            double hospitalbedsSum = 0.0;
            double hospitalcapacitySum = 0.0;
            double hospitalstaffSum = 0.0;
            double lifeexpectancySum = 0.0;
            foreach (DataPoint f in userData)
            {
                revenuefirstquarterSum += Math.Pow(f.RevenueFirstQuarter - revenuefirstquarterMean, 2);
                revenuesecondquarterSum += Math.Pow(f.RevenueSecondQuarter - revenuesecondquarterMean, 2);
                revenuethirdquarterSum += Math.Pow(f.RevenueThirdQuarter - revenuethirdquarterMean, 2);
                revenuefourthquarterSum += Math.Pow(f.RevenueFourthQuarter - revenuefourthquarterMean, 2);
                revenueyearSum += Math.Pow(f.RevenueYear - revenueyearMean, 2);
                averagesalarySum += Math.Pow(f.AverageSalary - averagesalaryMean, 2);
                investmentSum += Math.Pow(f.Investment - investmentMean, 2);
                employeesSum += Math.Pow(f.Employees - employeesMean, 2);
                workforceSum += Math.Pow(f.WorkForce - workforceMean, 2);
                enterprisesSum += Math.Pow(f.Enterprises - enterprisesMean, 2);
                unemploymentSum += Math.Pow(f.Unemployment - unemploymentMean, 2);
                povertylevelSum += Math.Pow(f.PovertyLevel - povertylevelMean, 2);
                morbiditySum += Math.Pow(f.Morbidity - morbidityMean, 2);
                hospitalbedsSum += Math.Pow(f.HospitalBeds - hospitalbedsMean, 2);
                hospitalcapacitySum += Math.Pow(f.HospitalCapacity - hospitalcapacityMean, 2);
                hospitalstaffSum += Math.Pow(f.HospitalStaff - hospitalstaffMean, 2);
                lifeexpectancySum += Math.Pow(f.LifeExpectancy - lifeexpectancyMean, 2);
            }
            double revenuefirstquarterSD = revenuefirstquarterSum / userData.Count;
            double revenuesecondquarterSD = revenuesecondquarterSum / userData.Count;
            double revenuethirdquarterSD = revenuethirdquarterSum / userData.Count;
            double revenuefourthquarterSD = revenuefourthquarterSum / userData.Count;
            double revenueyearSD = revenueyearSum / userData.Count;
            double averagesalarySD = averagesalarySum / userData.Count;
            double investmentSD = investmentSum / userData.Count;
            double employeesSD = employeesSum / userData.Count;
            double workforceSD = workforceSum / userData.Count;
            double enterprisesSD = enterprisesSum / userData.Count;
            double unemploymentSD = unemploymentSum / userData.Count;
            double povertylevelSD = povertylevelSum / userData.Count;
            double morbiditySD = morbiditySum / userData.Count;
            double hospitalbedsSD = hospitalbedsSum / userData.Count;
            double hospitalcapacitySD = hospitalcapacitySum / userData.Count;
            double hospitalstaffSD = hospitalstaffSum / userData.Count;
            double lifeexpectancySD = lifeexpectancySum / userData.Count;
            foreach (DataPoint f in userData)
            {
                normalizeData.Add(new DataPoint()
                {
                    NameRegion = f.NameRegion,
                    RevenueFirstQuarter = (f.RevenueFirstQuarter - revenuefirstquarterMean) / revenuefirstquarterSD,
                    RevenueSecondQuarter = (f.RevenueSecondQuarter - revenuesecondquarterMean) / revenuesecondquarterSD,
                    RevenueThirdQuarter = (f.RevenueThirdQuarter - revenuethirdquarterMean) / revenuethirdquarterSD,
                    RevenueFourthQuarter = (f.RevenueFourthQuarter - revenuefourthquarterMean) / revenuefourthquarterSD,
                    RevenueYear = (f.RevenueYear - revenueyearMean) / revenueyearSD,
                    AverageSalary = (f.AverageSalary - averagesalaryMean) / averagesalarySD,
                    Investment = (f.Investment - investmentMean) / investmentSD,
                    Employees = (f.Employees - employeesMean) / employeesSD,
                    WorkForce = (f.WorkForce - workforceMean) / workforceSD,
                    Enterprises = (f.Enterprises - enterprisesMean) / enterprisesSD,
                    Unemployment = (f.Unemployment - unemploymentMean) / unemploymentSD,
                    PovertyLevel = (f.PovertyLevel - povertylevelMean) / povertylevelSD,
                    Morbidity = (f.Morbidity - morbidityMean) / morbiditySD,
                    HospitalBeds = (f.HospitalBeds - hospitalbedsMean) / hospitalbedsSD,
                    HospitalCapacity = (f.HospitalCapacity - hospitalcapacityMean) / hospitalcapacitySD,
                    HospitalStaff = (f.HospitalStaff - hospitalstaffMean) / hospitalstaffSD,
                    LifeExpectancy = (f.LifeExpectancy - lifeexpectancyMean) / lifeexpectancySD,
                    Arrivals = f.Arrivals,
                    Departed = f.Departed,
                    MigrationGrowth = f.MigrationGrowth,
                    Cluster = 0
                });
            }
        }

        private void InitializeCentroids()
        {
            Random random = new Random(clusterCount);
            for (int i = 0; i < clusterCount; ++i)
            {
                normalizeData[i].Cluster = userData[i].Cluster = i;
            }
            for (int i = clusterCount; i < normalizeData.Count; i++)
            {
                normalizeData[i].Cluster = userData[i].Cluster = random.Next(0, clusterCount);
            }
        }

        private bool EmptyCluster(List<DataPoint> data)
        {
            var emptyCluster =
            data.GroupBy(s => s.Cluster).OrderBy(s => s.Key).Select(g => new { Cluster = g.Key, Count = g.Count() });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool UpdateDataPointMeans()
        {
            if (EmptyCluster(normalizeData)) return false;

            var groupToComputeMeans = normalizeData.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
            int clusterIndex = 0;
            double revenuefirstquarter = 0.0;
            double revenuesecondquarter = 0.0;
            double revenuethirdquarter = 0.0;
            double revenuefourthquarter = 0.0;
            double revenueyear = 0.0;
            double averagesalary = 0.0;
            double investment = 0.0;
            double employees = 0.0;
            double workforce = 0.0;
            double enterprises = 0.0;
            double unemployment = 0.0;
            double povertylevel = 0.0;
            double morbidity = 0.0;
            double hospitalbeds = 0.0;
            double hospitalcapacity = 0.0;
            double hospitalstaff = 0.0;
            double lifeexpectancy = 0.0;
            foreach (var item in groupToComputeMeans)
            {
                foreach (var f in item)
                {
                    revenuefirstquarter += f.RevenueFirstQuarter;
                    revenuesecondquarter += f.RevenueSecondQuarter;
                    revenuethirdquarter += f.RevenueThirdQuarter;
                    revenuefourthquarter += f.RevenueFourthQuarter;
                    revenueyear += f.RevenueYear;
                    averagesalary += f.AverageSalary;
                    investment += f.Investment;
                    employees += f.Employees;
                    workforce += f.WorkForce;
                    enterprises += f.Enterprises;
                    unemployment += f.Unemployment;
                    povertylevel += f.PovertyLevel;
                    morbidity += f.Morbidity;
                    hospitalbeds += f.HospitalBeds;
                    hospitalcapacity += f.HospitalCapacity;
                    hospitalstaff += f.HospitalStaff;
                    lifeexpectancy += f.LifeExpectancy;
                }
                clustersData[clusterIndex].RevenueFirstQuarter = revenuefirstquarter / item.Count();
                clustersData[clusterIndex].RevenueSecondQuarter = revenuesecondquarter / item.Count();
                clustersData[clusterIndex].RevenueThirdQuarter = revenuethirdquarter / item.Count();
                clustersData[clusterIndex].RevenueFourthQuarter = revenuefourthquarter / item.Count();
                clustersData[clusterIndex].RevenueYear = revenueyear / item.Count();
                clustersData[clusterIndex].AverageSalary = averagesalary / item.Count();
                clustersData[clusterIndex].Investment = investment / item.Count();
                clustersData[clusterIndex].Employees = employees / item.Count();
                clustersData[clusterIndex].WorkForce = workforce / item.Count();
                clustersData[clusterIndex].Enterprises = enterprises / item.Count();
                clustersData[clusterIndex].Unemployment = unemployment / item.Count();
                clustersData[clusterIndex].PovertyLevel = povertylevel / item.Count();
                clustersData[clusterIndex].Morbidity = morbidity / item.Count();
                clustersData[clusterIndex].HospitalBeds = hospitalbeds / item.Count();
                clustersData[clusterIndex].HospitalCapacity = hospitalcapacity / item.Count();
                clustersData[clusterIndex].HospitalStaff = hospitalstaff / item.Count();
                clustersData[clusterIndex].LifeExpectancy = lifeexpectancy / item.Count();
                clusterIndex++;
                revenuefirstquarter = 0.0;
                revenuesecondquarter = 0.0;
                revenuethirdquarter = 0.0;
                revenuefourthquarter = 0.0;
                revenueyear = 0.0;
                averagesalary = 0.0;
                investment = 0.0;
                employees = 0.0;
                workforce = 0.0;
                enterprises = 0.0;
                unemployment = 0.0;
                povertylevel = 0.0;
                morbidity = 0.0;
                hospitalbeds = 0.0;
                hospitalcapacity = 0.0;
                hospitalstaff = 0.0;
                lifeexpectancy = 0.0;
            }
            return true;
        }

        private double ElucidanDistance(DataPoint dataPoint1, DataPoint dataPoint2)
        {
            double distance = 0.0;
            distance = Math.Pow(dataPoint1.RevenueFirstQuarter - dataPoint2.RevenueFirstQuarter, 2);
            distance += Math.Pow(dataPoint1.RevenueSecondQuarter - dataPoint2.RevenueSecondQuarter, 2);
            distance += Math.Pow(dataPoint1.RevenueThirdQuarter - dataPoint2.RevenueThirdQuarter, 2);
            distance += Math.Pow(dataPoint1.RevenueFourthQuarter - dataPoint2.RevenueFourthQuarter, 2);
            distance += Math.Pow(dataPoint1.RevenueYear - dataPoint2.RevenueYear, 2);
            distance += Math.Pow(dataPoint1.AverageSalary - dataPoint2.AverageSalary, 2);
            distance += Math.Pow(dataPoint1.Investment - dataPoint2.Investment, 2);
            distance += Math.Pow(dataPoint1.Employees - dataPoint2.Employees, 2);
            distance += Math.Pow(dataPoint1.WorkForce - dataPoint2.WorkForce, 2);
            distance += Math.Pow(dataPoint1.Enterprises - dataPoint2.Enterprises, 2);
            distance += Math.Pow(dataPoint1.Unemployment - dataPoint2.Unemployment, 2);
            distance += Math.Pow(dataPoint1.PovertyLevel - dataPoint2.PovertyLevel, 2);
            distance += Math.Pow(dataPoint1.Morbidity - dataPoint2.Morbidity, 2);
            distance += Math.Pow(dataPoint1.HospitalBeds - dataPoint2.HospitalBeds, 2);
            distance += Math.Pow(dataPoint1.HospitalCapacity - dataPoint2.HospitalCapacity, 2);
            distance += Math.Pow(dataPoint1.HospitalStaff - dataPoint2.HospitalStaff, 2);
            distance += Math.Pow(dataPoint1.LifeExpectancy - dataPoint2.LifeExpectancy, 2);
            return Math.Sqrt(distance);
        }

        private int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }

        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[clusterCount];

            for (int i = 0; i < normalizeData.Count; ++i)
            {

                for (int k = 0; k < clusterCount; ++k)
                    distances[k] = ElucidanDistance(normalizeData[i], clustersData[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != normalizeData[i].Cluster)
                {
                    changed = true;
                    normalizeData[i].Cluster = userData[i].Cluster = newClusterId;
                }
            }
            if (changed == false)
                return false;
            if (EmptyCluster(normalizeData)) return false;
            return true;
        }

        private void Clear()
        {
            userData.Clear();
            normalizeData.Clear();
            clustersData.Clear();
            inputFileName = null;
        }
    }
}
