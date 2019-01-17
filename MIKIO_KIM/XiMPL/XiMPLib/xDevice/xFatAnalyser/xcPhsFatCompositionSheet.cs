using NPOI.HSSF.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xDevice.xFatAnalyser;

namespace XiMPLib.xDevice.xFatAnalyser {
    public class xcPhsFatCompositionSheet {
        private string[] ColumnHeader = { "idNumber",   "hospitalID",   "Date", "Time", "Bodytype", "Gender",   "Age",  "Height",   "Weight",   "BMI",      "BMR",      "FAT%",     "FATmass",  "FFM",      "TBW",      "Wbody", "Rleg", "Lleg", "Rarm", "Larm", "FAT%", "FATmass", "FFM", "PMM", "FAT%", "FATmass", "FFM", "PMM", "FAT%", "FATmass", "FFM", "PMM", "FAT%", "FATmass", "FFM", "PMM", "FAT%", "FATmass", "FFM", "PMM" };
        private string[] ColumnFormat = { "",           "",             "",     "",     "",         "",         "##",   "#,###",    "##0.0",    "##0.0",    "#,###",    "##0.0",    "##0.0",    "##0.0",    "##0.0",    "###", "###", "###", "###", "###", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0", "##0.0" };

        private string FilePath {
            get;
            set;
        }

        HSSFWorkbook WorkBook;
        HSSFSheet Sheet;

        public xcPhsFatCompositionSheet(string path) {
            FilePath = path;

            if (!Directory.GetParent(path).Exists) {
                Directory.CreateDirectory(Directory.GetParent(path).FullName);
            }

            if (File.Exists(path)) {
                try {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    WorkBook = new HSSFWorkbook(fs, true);
                    fs.Close();
                    Sheet = (HSSFSheet)WorkBook.GetSheet("Fat Composition Data");
                    if (Sheet.LastRowNum < 1)
                        initializeHeader();
                } catch {
                    initializeHeader();
                }
            } else {
                initializeHeader();
            }
        }

        public void addRow(string patientID, string hospitalID, xcFatCompositionRecord record) {
            var row = Sheet.CreateRow(Sheet.LastRowNum + 1);
            for (int i = 0; i < ColumnHeader.Length; i++) {
                var cell = row.CreateCell(i);

                HSSFCellStyle style = (HSSFCellStyle)WorkBook.CreateCellStyle();
                
                if (!string.IsNullOrEmpty(ColumnFormat[i])) {
                    row.Cells[i].SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                    style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    var format = (HSSFDataFormat)WorkBook.CreateDataFormat();
                    style.DataFormat = format.GetFormat(ColumnFormat[i]);
                } else {
                    style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                }
                
                row.Cells[i].CellStyle = style;
            }

            row.Cells[0].SetCellValue(patientID);
            row.Cells[1].SetCellValue(hospitalID);
            row.Cells[2].SetCellValue(record.MeasuredAt.ToString("yyyy-MM-dd"));
            row.Cells[3].SetCellValue(record.MeasuredAt.ToString("HH:mm"));
            row.Cells[4].SetCellValue(record.BodyType.ToString());
            row.Cells[5].SetCellValue(record.Gender.ToString());
            row.Cells[6].SetCellValue(record.BodyAge);
            row.Cells[7].SetCellValue(record.Height);
            row.Cells[8].SetCellValue(record.Weight);
            row.Cells[9].SetCellValue(record.BMI);
            row.Cells[10].SetCellValue(record.BmrByKCal);
            row.Cells[11].SetCellValue(record.BodyFatPercentage);
            row.Cells[12].SetCellValue(record.FatMass);
            row.Cells[13].SetCellValue(record.FatFreeMass);
            row.Cells[14].SetCellValue(record.BodyWaterMass);

            row.Cells[15].SetCellValue(record.ImpedanceWholeBody);
            row.Cells[16].SetCellValue(record.ImpedanceRightLeg);
            row.Cells[17].SetCellValue(record.ImpedanceLeftLeg);
            row.Cells[18].SetCellValue(record.ImpedanceRightArm);
            row.Cells[19].SetCellValue(record.ImpedanceLeftArm);

            row.Cells[20].SetCellValue(record.RightLegBodyFatPercentage);
            row.Cells[21].SetCellValue(record.RightLegFatMass);
            row.Cells[22].SetCellValue(record.RightLegFatFreeMass);
            row.Cells[23].SetCellValue(record.RightLegMuscleMass);

            row.Cells[24].SetCellValue(record.LeftLegBodyFatPercentage);
            row.Cells[25].SetCellValue(record.LeftLegFatMass);
            row.Cells[26].SetCellValue(record.LeftLegFatFreeMass);
            row.Cells[27].SetCellValue(record.LeftLegMuscleMass);

            row.Cells[28].SetCellValue(record.RightArmBodyFatPercentage);
            row.Cells[29].SetCellValue(record.RightArmFatMass);
            row.Cells[30].SetCellValue(record.RightArmFatFreeMass);
            row.Cells[31].SetCellValue(record.RightArmMuscleMass);

            row.Cells[32].SetCellValue(record.LeftArmBodyFatPercentage);
            row.Cells[33].SetCellValue(record.LeftArmFatMass);
            row.Cells[34].SetCellValue(record.LeftArmFatFreeMass);
            row.Cells[35].SetCellValue(record.LeftArmMuscleMass);

            row.Cells[36].SetCellValue(record.TrunkBodyFatPercentage);
            row.Cells[37].SetCellValue(record.TrunkFatMass);
            row.Cells[38].SetCellValue(record.TrunkFatFreeMass);
            row.Cells[39].SetCellValue(record.TrunkMuscleMass);

            save();
        }

        public void initializeHeader() {
            WorkBook = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
            Sheet = (HSSFSheet)WorkBook.CreateSheet("Fat Composition Data");

            var r0 = Sheet.CreateRow(0);
            for (int i = 0; i < ColumnHeader.Length; i++) {
                Sheet.SetColumnWidth(i, 256 * 15);
                //Sheet.SetDefaultColumnStyle(i, style);
                var cell = r0.CreateCell(i, NPOI.SS.UserModel.CellType.Blank);
                r0.Cells[i].CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                r0.Cells[i].CellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            }

            CellRangeAddress range;
            range = new CellRangeAddress(0, 0, 15, 19);
            Sheet.AddMergedRegion(range);
            r0.Cells[15].SetCellValue("Impedeance");

            range = new CellRangeAddress(0, 0, 20, 23);
            Sheet.AddMergedRegion(range);
            r0.Cells[20].SetCellValue("RightLeg");

            range = new CellRangeAddress(0, 0, 24, 27);
            Sheet.AddMergedRegion(range);
            r0.Cells[24].SetCellValue("LeftLeg");

            range = new CellRangeAddress(0, 0, 28, 31);
            Sheet.AddMergedRegion(range);
            r0.Cells[28].SetCellValue("RightArm");

            range = new CellRangeAddress(0, 0, 32, 35);
            Sheet.AddMergedRegion(range);
            r0.Cells[32].SetCellValue("LeftArm");

            range = new CellRangeAddress(0, 0, 36, 39);
            Sheet.AddMergedRegion(range);
            r0.Cells[36].SetCellValue("Trunk");

            var r1 = Sheet.CreateRow(1);
            for (int i = 0; i < ColumnHeader.Length; i++) {
                var cell = r1.CreateCell(i);
                cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                cell.SetCellValue(ColumnHeader[i]);
                cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                cell.CellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                cell.CellStyle.ShrinkToFit = false;
            }
        }

        public void save() {
            using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write)) {
                WorkBook.Write(fs);
                fs.Close();
            }
        }
    }
}
