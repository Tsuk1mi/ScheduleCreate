using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ScheduleCreate.Models;

namespace ScheduleCreate.Services
{
    public class ScheduleExportService : IScheduleExportService
    {
        public async Task ExportToExcelAsync(IEnumerable<ScheduleEntry> entries, string filePath)
        {
            await Task.Run(() =>
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Расписание");

                // Заголовки
                worksheet.Cell(1, 1).Value = "Дата";
                worksheet.Cell(1, 2).Value = "Время начала";
                worksheet.Cell(1, 3).Value = "Время окончания";
                worksheet.Cell(1, 4).Value = "Дисциплина";
                worksheet.Cell(1, 5).Value = "Преподаватель";
                worksheet.Cell(1, 6).Value = "Группы";
                worksheet.Cell(1, 7).Value = "Аудитория";
                worksheet.Cell(1, 8).Value = "Тип занятия";

                // Данные
                var row = 2;
                foreach (var entry in entries)
                {
                    worksheet.Cell(row, 1).Value = entry.Date.ToShortDateString();
                    worksheet.Cell(row, 2).Value = entry.StartTime.ToString(@"hh\:mm");
                    worksheet.Cell(row, 3).Value = entry.EndTime.ToString(@"hh\:mm");
                    worksheet.Cell(row, 4).Value = entry.Discipline?.Name;
                    worksheet.Cell(row, 5).Value = entry.Teacher?.FullName;
                    worksheet.Cell(row, 6).Value = string.Join(", ", entry.Groups.Select(g => g.Name));
                    worksheet.Cell(row, 7).Value = entry.Auditorium?.Number;
                    worksheet.Cell(row, 8).Value = entry.Type.ToString();
                    row++;
                }

                // Форматирование
                var range = worksheet.Range(1, 1, row - 1, 8);
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            });
        }

        public async Task<IEnumerable<ScheduleEntry>> ImportFromExcelAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                var entries = new List<ScheduleEntry>();
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RowsUsed().Skip(1); // Пропускаем заголовки
                foreach (var row in rows)
                {
                    var entry = new ScheduleEntry
                    {
                        Date = DateTime.Parse(row.Cell(1).Value.ToString()),
                        StartTime = TimeSpan.Parse(row.Cell(2).Value.ToString()),
                        EndTime = TimeSpan.Parse(row.Cell(3).Value.ToString()),
                        Discipline = new Discipline { Name = row.Cell(4).Value.ToString() },
                        Teacher = new Teacher { FullName = row.Cell(5).Value.ToString() },
                        Groups = row.Cell(6).Value.ToString()
                            .Split(',')
                            .Select(g => new Group { Name = g.Trim() })
                            .ToList(),
                        Auditorium = new Auditorium { Number = row.Cell(7).Value.ToString() },
                        Type = (ScheduleEntryType)Enum.Parse(typeof(ScheduleEntryType), row.Cell(8).Value.ToString())
                    };
                    entries.Add(entry);
                }

                return entries;
            });
        }

        public async Task ExportToPdfAsync(IEnumerable<ScheduleEntry> entries, string filePath)
        {
            await Task.Run(() =>
            {
                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Заголовок
                document.Add(new Paragraph("Расписание занятий")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20));

                // Таблица
                var table = new Table(8);
                table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                // Заголовки
                table.AddHeaderCell("Дата");
                table.AddHeaderCell("Время начала");
                table.AddHeaderCell("Время окончания");
                table.AddHeaderCell("Дисциплина");
                table.AddHeaderCell("Преподаватель");
                table.AddHeaderCell("Группы");
                table.AddHeaderCell("Аудитория");
                table.AddHeaderCell("Тип занятия");

                // Данные
                foreach (var entry in entries)
                {
                    table.AddCell(entry.Date.ToShortDateString());
                    table.AddCell(entry.StartTime.ToString(@"hh\:mm"));
                    table.AddCell(entry.EndTime.ToString(@"hh\:mm"));
                    table.AddCell(entry.Discipline?.Name);
                    table.AddCell(entry.Teacher?.FullName);
                    table.AddCell(string.Join(", ", entry.Groups.Select(g => g.Name)));
                    table.AddCell(entry.Auditorium?.Number);
                    table.AddCell(entry.Type.ToString());
                }

                document.Add(table);
            });
        }
    }
} 