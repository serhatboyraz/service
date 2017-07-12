using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Data.Models;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Anketbaz.Database.Log;

namespace Anketbaz.Data
{
    public class Exporter
    {
        public static string PollExportExcel(long ownerid, string ownerType, long pollId)
        {
            Log.Info("GetData Started");

            poll pollMaster = EntityConnectionService.Poll.GetSingle(
                x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId));

            if (pollMaster == null)
                return Helper.GetResult(false, "0x0012");

            IList<guest> guestList = EntityConnectionService.Guest.GetList(
                x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId));

            IList<guestanswer> guestAnswers = EntityConnectionService.GuestAnswer.GetList(
                x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId));

            IList<question> pollQuestions = EntityConnectionService.Question.GetList(
                    x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId))
                .OrderBy(x => x.questionid)
                .ToList();

            IList<answer> pollAnswers = EntityConnectionService.Answer.GetList(
                x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId));

            Log.Info("GetData Ended");
            Log.Info("Export Started!");

            string base64Data;

            DataTable dataTable = new DataTable(); //verileri kaydetmek icin datatable

            List<dynamic> fieldList = JsonConvert.DeserializeObject<List<dynamic>>(pollMaster.fielddata); //Bu anketin tum alanlarini parcala

            List<object> headerParams = new List<object>(); //basliklar icin headerparameters

            Log.Info("Header Creating!");
            for (int i = 0; i < fieldList.Count; i++) //tum alanlar icinde don ve kolonlari belirle
            {
                dataTable.Columns.Add(fieldList[i].FieldCode.ToString(), typeof(string)); //kolonlarin adlari
                headerParams.Add(fieldList[i].FieldTitle.ToString()); //kolonlarin icerikleri
            }

            for (int i = 0; i < pollQuestions.Count; i++) //tum sorular icinde don ve kolonlari belirle
            {
                dataTable.Columns.Add(pollQuestions[i].questionid.ToString(), typeof(string)); //kolonlarin adlari
                headerParams.Add(pollQuestions[i].content); //kolonlarin icerikleri
            }
            dataTable.Rows.Add(headerParams.ToArray()); //basliklari tabloya ekle

            Log.Info("Header Created!");

            Log.Info("Guests Creating!");
            for (int i = 0; i < guestList.Count; i++)
            {

                long guestId = guestList[i].guestid; //anket yapilan kisinin idsi
                Log.Info(string.Format("Start:{0}", guestId));

                List<guestanswer> localGuestAnswers =
                            guestAnswers.Where(
                                x => x.guestid.Equals(guestId)).ToList();
                List<object> guestData = new List<object>();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (j < fieldList.Count)
                    {
                        //field icin burasi
                        List<dynamic> guestFieldData = JsonConvert.DeserializeObject<List<dynamic>>(guestList[i].fielddata); //anket yapilan kisinin sadece alanlari
                        for (int k = 0; k < guestFieldData.Count; k++)//bu anketi yapan kullanicinin alanlarini almak icin don
                        {
                            if (dataTable.Columns[j].ColumnName == guestFieldData[k].FieldCode.ToString()) //bu alanin kodu kolonun koduna esitse yani veri buraya gelmeli ise
                            {
                                guestData.Add(guestFieldData[k].FieldValue.ToString());  //bulunan veriyi anket yapilan kullanicinin verilerinin icine ekle.
                                break;//ilgili alan bulundugu icin donguden cikilabilir.
                            }
                        }
                    }
                    else
                    {
                        //soru icin burasi

                        localGuestAnswers =
                            localGuestAnswers.Where(x => x.questionid.ToString() == (dataTable.Columns[j].ColumnName)).ToList();
                        string cellData = string.Empty;
                        
                        for (int k = 0; k < localGuestAnswers.Count; k++)
                        {
                            answer cellAnswer =
                                pollAnswers.FirstOrDefault(x => x.answerid.Equals(localGuestAnswers[k].answerid));
                            if (cellAnswer != null)
                                cellData += cellAnswer.content + ",";
                            guestAnswers.Remove(localGuestAnswers[k]);
                        }

                        cellData = cellData.Trim(',');
                        guestData.Add(cellData);

                    }
                }

                dataTable.Rows.Add(guestData.ToArray());//tum satirlari datatable a ekle

                Log.Info(string.Format("End:{0}", guestId));
            }
            Log.Info("Guests Created!");
            Log.Info("Ms Creating!");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet("Sheet1");
                ICreationHelper cH = wb.GetCreationHelper();

                var font = wb.CreateFont();
                font.FontHeightInPoints = 11;
                font.FontName = "Calibri";
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i);
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        if (i == 0) cell.CellStyle.SetFont(font);
                        cell.SetCellValue(cH.CreateRichTextString(dataTable.Rows[i].ItemArray[j].ToString()));
                    }
                }
                wb.Write(memoryStream);
                base64Data = Convert.ToBase64String(memoryStream.ToArray());
            }

            Log.Info("Export Ended");

            return Helper.GetResult(true, base64Data);

        }
    }
}
