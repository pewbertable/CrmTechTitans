using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CrmTechTitans.Services
{
    public class ExcelExportService
    {
        public byte[] GenerateExcelPackage(
            IEnumerable<Dictionary<string, object>> data,
            List<string> selectedFields,
            string sheetName = "Sheet1",
            string reportTitle = "Report")
        {
            // Set the license context for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);

                // --- Title ---
                workSheet.Cells[1, 1].Value = reportTitle;
                using (ExcelRange title = workSheet.Cells[1, 1, 1, selectedFields.Count])
                {
                    title.Merge = true;
                    title.Style.Font.Bold = true;
                    title.Style.Font.Size = 18;
                    title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    title.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    title.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150)); // Dark blue background
                    title.Style.Font.Color.SetColor(Color.White);
                }

                // --- Timestamp ---
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // Consider making timezone configurable if needed
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                workSheet.Cells[2, 1].Value = "Created: " + localDate.ToString("MMMM d, yyyy h:mm tt");
                using (ExcelRange dateRange = workSheet.Cells[2, 1, 2, selectedFields.Count])
                {
                    dateRange.Merge = true;
                    dateRange.Style.Font.Bold = true;
                    dateRange.Style.Font.Size = 11;
                    dateRange.Style.Font.Italic = true;
                    dateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    dateRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    dateRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // --- Headers ---
                int headerRow = 3;
                for (int col = 0; col < selectedFields.Count; col++)
                {
                    workSheet.Cells[headerRow, col + 1].Value = selectedFields[col]; // Use selected field names as headers
                }

                // Style Headers
                using (ExcelRange headers = workSheet.Cells[headerRow, 1, headerRow, selectedFields.Count])
                {
                    headers.Style.Font.Bold = true;
                    headers.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headers.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 242)); // Light blue background
                    headers.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    headers.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    headers.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    headers.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    headers.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // --- Data ---
                int dataStartRow = headerRow + 1;
                if (data != null && data.Any())
                {
                    int currentRow = dataStartRow;
                    foreach (var item in data)
                    {
                        for (int col = 0; col < selectedFields.Count; col++)
                        {
                            string fieldName = selectedFields[col];
                            object value = item.ContainsKey(fieldName) ? item[fieldName] : null;

                            // Basic Formatting / Handling Nulls
                            if (value is DateTime dt)
                            {
                                workSheet.Cells[currentRow, col + 1].Value = dt;
                                workSheet.Cells[currentRow, col + 1].Style.Numberformat.Format = "yyyy-mm-dd"; // Apply date format
                            }
                            else
                            {
                                workSheet.Cells[currentRow, col + 1].Value = value ?? "N/A"; // Handle nulls
                            }
                        }
                        currentRow++;
                    }

                    // --- Styling Data Area ---
                    int lastRow = currentRow - 1;
                    int lastCol = selectedFields.Count;

                    // Apply alternating row colors
                    for (int dataRow = dataStartRow; dataRow <= lastRow; dataRow++)
                    {
                        using (ExcelRange rowRange = workSheet.Cells[dataRow, 1, dataRow, lastCol])
                        {
                            // Apply light gray to even data rows (adjusting for header rows)
                            if ((dataRow - dataStartRow) % 2 == 1)
                            {
                                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242)); // Very light gray
                            }

                            // Apply borders to all data cells
                            rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            // Vertical alignment
                            rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }

                     // AutoFit Columns after data is added
                    workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                    // Example: Specific Column Formatting (can be expanded)
                    for (int col = 0; col < selectedFields.Count; col++)
                     {
                        string fieldName = selectedFields[col];
                        if (fieldName.Contains("Date") || fieldName.Contains("Since")) // Basic date column detection
                        {
                            workSheet.Column(col + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                         // Add more specific formatting (like URLs, currency, etc.) if needed
                     }

                }
                else // Handle case with no data
                {
                     workSheet.Cells[dataStartRow, 1].Value = "No data available for the selected criteria.";
                     workSheet.Cells[dataStartRow, 1, dataStartRow, selectedFields.Count].Merge = true;
                     workSheet.Cells[dataStartRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }


                // Return the Excel file as a byte array
                return excel.GetAsByteArray();
            }
        }
    }
}
