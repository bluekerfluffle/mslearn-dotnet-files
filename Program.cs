using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesFiles = FindFiles(storesDirectory);
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);
var salesTotal = CalculateSalesTotal(salesFiles); 
File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();
    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}
double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    return salesTotal;

}
void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, double totalSales, string outputDirectory)
{
    StringBuilder reportBuilder = new StringBuilder();
    reportBuilder.AppendLine("Sales Summary");
    reportBuilder.AppendLine("----------------------------");
    reportBuilder.AppendLine($" Total Sales: {totalSales.ToString("C")}");
    reportBuilder.AppendLine();
    reportBuilder.AppendLine(" Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double salesAmount = data?.Total ?? 0;
        reportBuilder.AppendLine($" {Path.GetFileName(file)}: {salesAmount.ToString("C")}");
    }

    var reportFile = Path.Combine(outputDirectory, "SalesSummary.txt");
    File.WriteAllText(reportFile, reportBuilder.ToString());
}

GenerateSalesSummaryReport(salesFiles, salesTotal, salesTotalDir);

record SalesData (double Total);

