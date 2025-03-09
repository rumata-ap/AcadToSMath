using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;

using netDxf;
using netDxf.Entities;
using netDxf.Tables;

using SMath.Manager;
using SMath.Math;
using SMath.Math.Numeric;

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AcadToSMath
{
   public class AcadToSMath : IPluginHandleEvaluation, IPluginLowLevelEvaluationFast
   {
      AssemblyInfo[] asseblyInfos;

      public void Dispose()
      {
      }

      public TermInfo[] GetTermsHandled(SessionProfile sessionProfile)
      {
         //functions definitions
         return new TermInfo[]
                  {
                  new TermInfo("GetAcPlines", TermType.Function,
                  "(1:слой, 2:масштаб единиц чертежа, 3:замкнуть полилинии) - " +
                  "Получает вершины полилиний из активного документа AutoCAD, возвращает массив с координатами вершин полилиний.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetAcCircles", TermType.Function,
                  "(1:слой, 2:масштаб единиц чертежа) - " +
                  "Получает  координаты центров, площади и диамерты окружностей из активного документа AutoCAD, возвращает массив с данными окружностей.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetAcTextes", TermType.Function,
                  "(1:слой, 2:масштаб единиц чертежа) - " +
                  "Получает  координаты точек вставки и содержимое однострочных текстов из активного документа AutoCAD, возвращает массив с данными текста.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetAcPoints", TermType.Function,
                  "(1:слой, 2:масштаб единиц чертежа) - " +
                  "Получает  координаты точек из активного документа AutoCAD, возвращает массив с координатами точек.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetAcLines", TermType.Function,
                  "(1:слой, 2:масштаб единиц чертежа) - " +
                  "Получает  координаты точек отрезков и их длину из активного документа AutoCAD, возвращает массив с данными отрезков.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetDxfLines", TermType.Function,
                  "(1:путь к dxf-файлу, 2:слой, 3:масштаб единиц чертежа) - " +
                  "Получает  координаты точек отрезков и их длину из dxf-файла, возвращает массив с данными отрезков.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String),
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetDxfPoints", TermType.Function,
                  "(1:путь к dxf-файлу, 2:слой, 3:масштаб единиц чертежа) - " +
                  "Получает  координаты точек из dxf-файла, возвращает массив с координатами точек.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String),
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetDxfTextes", TermType.Function,
                  "(1:путь к dxf-файлу, 2:слой, 3:масштаб единиц чертежа) - " +
                  "Получает  координаты точек вставки и содержимое однострочных текстов из dxf-файла, возвращает массив с данными текста.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String),
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetDxfCircles", TermType.Function,
                  "(1:путь к dxf-файлу, 2:слой, 3:масштаб единиц чертежа) - " +
                  "Получает  координаты центров, площади и диамерты окружностей из dxf-файла, возвращает массив с данными окружностей.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String),
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetDxfReinfBars", TermType.Function,
                  "(1:путь к dxf-файлу, 2:маска слоя, 3:масштаб единиц чертежа) - " +
                  "Получает  координаты центров, площади и диамерты окружностей из dxf-файла, возвращает массив с данными окружностей.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String),
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  new TermInfo("GetDxfPlines", TermType.Function,
                  "(1:путь к dxf-файлу, 2:слой, 3:масштаб единиц чертежа) - " +
                  "Получает вершины полилиний из dxf-файла, возвращает массив с координатами вершин полилиний.",
                  FunctionSections.Files, true,
                  new ArgumentInfo(ArgumentSections.String),
                  new ArgumentInfo(ArgumentSections.String, true),
                  new ArgumentInfo(ArgumentSections.RealNumber, true)),

                  };
      }

      public void Initialize()
      {
         asseblyInfos = new AssemblyInfo[] { new AssemblyInfo("SMath Studio", new Version(0, 99), new Guid("86af75bc-5bae-45ac-ba9e-e80940da6060")) };

         AppDomain currentDomain = AppDomain.CurrentDomain;
         currentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

         System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
         {
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < asms.Length; ++i)
            {
               if (asms[i].FullName == args.Name)
                  return asms[i];
            }
            return null;
         }
      }


      public bool TryEvaluateExpression(Entry value, Store context, out Entry result)
      {
         //GetAcPlines
         if (value.Type == TermType.Function && value.Text == "GetAcPlines")
         {
            List<Term> answer = new List<Term>();

            string lay = "#";
            double scale = 1;
            int isClose = 0;
            if (TermsConverter.DecodeText(value.Items[0].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[0], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[1], context);
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") isClose = Utilites.Entry2Int(value.Items[2], context);


            const string progID = "AutoCAD.Application";
            AcadApplication acApp = null;

            try
            {
               acApp = (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
               answer.AddRange(TermsConverter.ToTerms("\"AutoCAD не запущен\""));

               result = Entry.Create(answer.ToArray());
               return true;
            }
            if (acApp != null)
            {
               // By the time this is reached AutoCAD is fully
               // functional and can be interacted with through code
               acApp.Visible = true;
               AcadModelSpace ms = null;
               try
               {
                  ms = acApp.ActiveDocument.ModelSpace;
                  List<AcadLWPolyline> polylines = new List<AcadLWPolyline>();
                  if (lay == "#")
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbPolyline") polylines.Add(ms.Item(i) as AcadLWPolyline);
                     }
                  }
                  else
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbPolyline" && ms.Item(i).Layer == lay) polylines.Add(ms.Item(i) as AcadLWPolyline);
                     }
                  }
                  if (polylines.Count == 0)
                  {
                     answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной полилинии на заданном слое\""));
                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
                  else
                  {
                     int plc = polylines.Count;
                     for (int i = 0; i < plc; i++)
                     {
                        AcadLWPolyline pline = polylines[i];
                        double[] crds = (double[])polylines[i].Coordinates;
                        int crdsc = crds.Length / 2;
                        //List<double[]> crd = new List<double[]>(crdsc);
                        foreach (double item in crds) answer.AddRange(new TNumber(item * scale).obj.ToTerms());
                        if (isClose == 1)
                        {
                           answer.AddRange(new TNumber(crds[0] * scale).obj.ToTerms());
                           answer.AddRange(new TNumber(crds[1] * scale).obj.ToTerms());
                           answer.AddRange(TermsConverter.ToTerms((crdsc + 1).ToString()));
                           answer.AddRange(TermsConverter.ToTerms(2.ToString()));
                           answer.Add(new Term(Functions.Mat, TermType.Function, 4 + crdsc * 2));
                        }
                        else
                        {
                           answer.AddRange(TermsConverter.ToTerms(crdsc.ToString()));
                           answer.AddRange(TermsConverter.ToTerms(2.ToString()));
                           answer.Add(new Term(Functions.Mat, TermType.Function, 2 + crdsc * 2));
                        }
                     }

                     if (plc > 1)
                     {
                        answer.AddRange(TermsConverter.ToTerms(1.ToString()));
                        answer.AddRange(TermsConverter.ToTerms(plc.ToString()));
                        answer.Add(new Term(Functions.Mat, TermType.Function, 2 + plc));
                     }

                     result = Entry.Create(answer.ToArray());
                     return true;
                  }

               }
               catch
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Нет ни одного открытого документа AutoCAD\""));

                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetAcCircles
         if (value.Type == TermType.Function && value.Text == "GetAcCircles")
         {
            List<Term> answer = new List<Term>();

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[0].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[0], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[1], context);


            const string progID = "AutoCAD.Application";
            AcadApplication acApp = null;

            try
            {
               acApp = (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
               answer.AddRange(TermsConverter.ToTerms("\"AutoCAD не запущен\""));

               result = Entry.Create(answer.ToArray());
               return true;
            }
            if (acApp != null)
            {
               // By the time this is reached AutoCAD is fully
               // functional and can be interacted with through code
               acApp.Visible = true;
               AcadModelSpace ms = null;
               try
               {
                  ms = acApp.ActiveDocument.ModelSpace;
                  List<AcadCircle> circles = new List<AcadCircle>();
                  if (lay == "#")
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbCircle") circles.Add(ms.Item(i) as AcadCircle);
                     }
                  }
                  else
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbCircle" && ms.Item(i).Layer == lay) circles.Add(ms.Item(i) as AcadCircle);
                     }
                  }
                  if (circles.Count == 0)
                  {
                     answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной окружности на заданном слое\""));
                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
                  else
                  {
                     int cc = circles.Count;
                     for (int i = 0; i < cc; i++)
                     {
                        AcadCircle c = circles[i];
                        double[] cen = (double[])c.Center;
                        answer.AddRange(new TNumber(cen[0] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(cen[1] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(c.Area * scale * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(c.Radius * scale * 2).obj.ToTerms());
                     }
                     answer.AddRange(TermsConverter.ToTerms(cc.ToString()));
                     answer.AddRange(TermsConverter.ToTerms(4.ToString()));
                     answer.Add(new Term(Functions.Mat, TermType.Function, 2 + cc * 4));

                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
               }
               catch
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Нет ни одного открытого документа AutoCAD\""));

                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetAcPoints
         if (value.Type == TermType.Function && value.Text == "GetAcPoints")
         {
            List<Term> answer = new List<Term>();

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[0].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[0], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[1], context);


            const string progID = "AutoCAD.Application";
            AcadApplication acApp = null;

            try
            {
               acApp = (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
               answer.AddRange(TermsConverter.ToTerms("\"AutoCAD не запущен\""));

               result = Entry.Create(answer.ToArray());
               return true;
            }
            if (acApp != null)
            {
               // By the time this is reached AutoCAD is fully
               // functional and can be interacted with through code
               acApp.Visible = true;
               AcadModelSpace ms = null;
               try
               {
                  ms = acApp.ActiveDocument.ModelSpace;
                  List<AcadPoint> points = new List<AcadPoint>();
                  if (lay == "#")
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbPoint") points.Add(ms.Item(i) as AcadPoint);
                     }
                  }
                  else
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbPoint" && ms.Item(i).Layer == lay) points.Add(ms.Item(i) as AcadPoint);
                     }
                  }
                  if (points.Count == 0)
                  {
                     answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной точки на заданном слое\""));
                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
                  else
                  {
                     int pc = points.Count;
                     for (int i = 0; i < pc; i++)
                     {
                        AcadPoint p = points[i];
                        double[] cen = (double[])p.Coordinates;
                        answer.AddRange(new TNumber(cen[0] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(cen[1] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(cen[2] * scale).obj.ToTerms());
                     }
                     answer.AddRange(TermsConverter.ToTerms(pc.ToString()));
                     answer.AddRange(TermsConverter.ToTerms(3.ToString()));
                     answer.Add(new Term(Functions.Mat, TermType.Function, 2 + pc * 3));

                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
               }
               catch
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Нет ни одного открытого документа AutoCAD\""));

                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetAcTextes
         if (value.Type == TermType.Function && value.Text == "GetAcTextes")
         {
            List<Term> answer = new List<Term>();

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[0].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[0], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[1], context);


            const string progID = "AutoCAD.Application";
            AcadApplication acApp = null;

            try
            {
               acApp = (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
               answer.AddRange(TermsConverter.ToTerms("\"AutoCAD не запущен\""));

               result = Entry.Create(answer.ToArray());
               return true;
            }
            if (acApp != null)
            {
               // By the time this is reached AutoCAD is fully
               // functional and can be interacted with through code
               acApp.Visible = true;
               AcadModelSpace ms = null;
               try
               {
                  ms = acApp.ActiveDocument.ModelSpace;
                  List<AcadText> points = new List<AcadText>();
                  if (lay == "#")
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbText") points.Add(ms.Item(i) as AcadText);
                     }
                  }
                  else
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbText" && ms.Item(i).Layer == lay) points.Add(ms.Item(i) as AcadText);
                     }
                  }
                  if (points.Count == 0)
                  {
                     answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного текста на заданном слое\""));
                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
                  else
                  {
                     int pc = points.Count;
                     for (int i = 0; i < pc; i++)
                     {
                        AcadText p = points[i];
                        double[] cen = (double[])p.InsertionPoint;
                        answer.AddRange(new TNumber(cen[0] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(cen[1] * scale).obj.ToTerms());
                        answer.AddRange(TermsConverter.ToTerms("\"" + p.TextString + "\""));
                     }
                     answer.AddRange(TermsConverter.ToTerms(pc.ToString()));
                     answer.AddRange(TermsConverter.ToTerms(3.ToString()));
                     answer.Add(new Term(Functions.Mat, TermType.Function, 2 + pc * 3));

                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
               }
               catch
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Нет ни одного открытого документа AutoCAD\""));

                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetAcLines
         if (value.Type == TermType.Function && value.Text == "GetAcLines")
         {
            List<Term> answer = new List<Term>();

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[0].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[0], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[1], context);


            const string progID = "AutoCAD.Application";
            AcadApplication acApp = null;

            try
            {
               acApp = (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
               answer.AddRange(TermsConverter.ToTerms("\"AutoCAD не запущен\""));

               result = Entry.Create(answer.ToArray());
               return true;
            }
            if (acApp != null)
            {
               // By the time this is reached AutoCAD is fully
               // functional and can be interacted with through code
               acApp.Visible = true;
               AcadModelSpace ms = null;
               try
               {
                  ms = acApp.ActiveDocument.ModelSpace;
                  List<AcadLine> points = new List<AcadLine>();
                  if (lay == "#")
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbLine") points.Add(ms.Item(i) as AcadLine);
                     }
                  }
                  else
                  {
                     for (int i = 0; i < ms.Count; i++)
                     {
                        if (ms.Item(i).ObjectName == "AcDbLine" && ms.Item(i).Layer == lay) points.Add(ms.Item(i) as AcadLine);
                     }
                  }
                  if (points.Count == 0)
                  {
                     answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного отрезка на заданном слое\""));
                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
                  else
                  {
                     int pc = points.Count;
                     for (int i = 0; i < pc; i++)
                     {
                        AcadLine p = points[i];
                        double[] sp = (double[])p.StartPoint;
                        double[] ep = (double[])p.EndPoint;
                        double len = p.Length;
                        answer.AddRange(new TNumber(sp[0] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(sp[1] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(ep[0] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(ep[1] * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(len * scale).obj.ToTerms());
                     }
                     answer.AddRange(TermsConverter.ToTerms(pc.ToString()));
                     answer.AddRange(TermsConverter.ToTerms(5.ToString()));
                     answer.Add(new Term(Functions.Mat, TermType.Function, 2 + pc * 5));

                     result = Entry.Create(answer.ToArray());
                     return true;
                  }
               }
               catch
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Нет ни одного открытого документа AutoCAD\""));

                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetDxfLines
         if (value.Type == TermType.Function && value.Text == "GetDxfLines")
         {
            List<Term> answer = new List<Term>();

            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            string filepath = TermsConverter.DecodeText(arg1.Text).Trim('"');
            int indexOfChar = filepath.IndexOf(':');
            if (indexOfChar == -1) filepath = Utilites.CurrentPath(context.FileName) + "\\" + filepath;
            DxfDocument dxf = DxfDocument.Load(filepath);

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[1], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[2], context);

            if (dxf.Lines.Count() == 0)
            {
               answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного отрезка на заданном слое\""));
               result = Entry.Create(answer.ToArray());
               return true;
            }
            else
            {
               List<Line> lines = new List<Line>();
               foreach (Line item in dxf.Lines)
               {
                  if (lay != "#" && item.Layer.Name == lay) lines.Add(item);
                  else if (lay == "#") lines.Add(item);
                  else continue;
               }
               if (lines.Count == 0)
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного отрезка на заданном слое\""));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
               else
               {
                  foreach (Line item in lines)
                  {
                     answer.AddRange(new TNumber(item.StartPoint.X * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.StartPoint.Y * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.EndPoint.X * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.EndPoint.Y * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(Math.Sqrt(Math.Pow(item.EndPoint.X - item.StartPoint.X, 2) +
                        Math.Pow(item.EndPoint.Y - item.StartPoint.Y, 2)) * scale).obj.ToTerms());
                  }
                  answer.AddRange(TermsConverter.ToTerms(lines.Count.ToString()));
                  answer.AddRange(TermsConverter.ToTerms(5.ToString()));
                  answer.Add(new Term(Functions.Mat, TermType.Function, 2 + lines.Count * 5));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetDxfPoints
         if (value.Type == TermType.Function && value.Text == "GetDxfPoints")
         {
            List<Term> answer = new List<Term>();

            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            string filepath = TermsConverter.DecodeText(arg1.Text).Trim('"');
            int indexOfChar = filepath.IndexOf(':');
            if (indexOfChar == -1) filepath = Utilites.CurrentPath(context.FileName) + "\\" + filepath;
            DxfDocument dxf = DxfDocument.Load(filepath);

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[1], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[2], context);

            if (dxf.Lines.Count() == 0)
            {
               answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной точки на заданном слое\""));
               result = Entry.Create(answer.ToArray());
               return true;
            }
            else
            {
               List<Point> lines = new List<Point>();
               foreach (Point item in dxf.Points)
               {
                  if (lay != "#" && item.Layer.Name == lay) lines.Add(item);
                  else if (lay == "#") lines.Add(item);
                  else continue;
               }
               if (lines.Count == 0)
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного отрезка на заданном слое\""));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
               else
               {
                  foreach (Point item in lines)
                  {
                     answer.AddRange(new TNumber(item.Position.X * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.Position.Y * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.Position.Z * scale).obj.ToTerms());
                  }
                  answer.AddRange(TermsConverter.ToTerms(lines.Count.ToString()));
                  answer.AddRange(TermsConverter.ToTerms(3.ToString()));
                  answer.Add(new Term(Functions.Mat, TermType.Function, 2 + lines.Count * 3));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetDxfTextes
         if (value.Type == TermType.Function && value.Text == "GetDxfTextes")
         {
            List<Term> answer = new List<Term>();

            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            string filepath = TermsConverter.DecodeText(arg1.Text).Trim('"');
            int indexOfChar = filepath.IndexOf(':');
            if (indexOfChar == -1) filepath = Utilites.CurrentPath(context.FileName) + "\\" + filepath;
            DxfDocument dxf = DxfDocument.Load(filepath);

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[1], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[2], context);

            if (dxf.Lines.Count() == 0)
            {
               answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного текста на заданном слое\""));
               result = Entry.Create(answer.ToArray());
               return true;
            }
            else
            {
               List<Text> lines = new List<Text>();
               foreach (Text item in dxf.Texts)
               {
                  if (lay != "#" && item.Layer.Name == lay) lines.Add(item);
                  else if (lay == "#") lines.Add(item);
                  else continue;
               }
               if (lines.Count == 0)
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одного текста на заданном слое\""));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
               else
               {
                  foreach (Text item in lines)
                  {
                     answer.AddRange(new TNumber(item.Position.X * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.Position.Y * scale).obj.ToTerms());
                     answer.AddRange(TermsConverter.ToTerms("\"" + item.Value + "\""));
                  }
                  answer.AddRange(TermsConverter.ToTerms(lines.Count.ToString()));
                  answer.AddRange(TermsConverter.ToTerms(3.ToString()));
                  answer.Add(new Term(Functions.Mat, TermType.Function, 2 + lines.Count * 3));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetDxfReinfBars
         if (value.Type == TermType.Function && value.Text == "GetDxfReinfBars")
         {
            List<Term> answer = new List<Term>();

            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            string filepath = TermsConverter.DecodeText(arg1.Text).Trim('"');
            int indexOfChar = filepath.IndexOf(':');
            if (indexOfChar == -1) filepath = Utilites.CurrentPath(context.FileName) + "\\" + filepath;
            DxfDocument dxf = DxfDocument.Load(filepath);

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") lay = TermsConverter.DecodeText(value.Items[1].Text).Trim('"');
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[2], context);

            if (dxf.Circles.Count() == 0)
            {
               answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной окружности на заданном слое\""));
               result = Entry.Create(answer.ToArray());
               return true;
            }
            else
            {
               List<string> layers = new List<string>();
               foreach (Layer item in dxf.Layers)
               {
                  if (item.Name.IndexOf(lay) != -1) layers.Add(item.Name);
               }
               if (layers.Count == 0)
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной окружности на заданном слое\""));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
               else
               {
                  Dictionary<string, List<Circle>> rbgs = new Dictionary<string, List<Circle>>(layers.Count);
                  foreach (string ls in layers)
                  {
                     rbgs.Add(ls, new List<Circle>());
                     List<Circle> circles = new List<Circle>();
                     foreach (Circle item in dxf.Circles)
                     {
                        if (item.Layer.Name == ls) rbgs[ls].Add(item);
                     }
                  }

                  int cnt = rbgs.Count;
                  foreach (List<Circle> cs in rbgs.Values)
                  {
                     if (cs.Count != 0)
                     {
                        foreach (Circle c in cs)
                        {
                           answer.AddRange(new TNumber(c.Center.X * scale).obj.ToTerms());
                           answer.AddRange(new TNumber(c.Center.Y * scale).obj.ToTerms());
                           answer.AddRange(new TNumber(2 * c.Radius * scale).obj.ToTerms());
                           answer.AddRange(new TNumber(Math.Pow(c.Radius * scale, 2) * Math.PI).obj.ToTerms());
                           answer.AddRange(TermsConverter.ToTerms($"\"{c.Layer.Name}\""));
                        }
                        answer.AddRange(TermsConverter.ToTerms(cs.Count.ToString()));
                        answer.AddRange(TermsConverter.ToTerms(5.ToString()));
                        answer.Add(new Term(Functions.Mat, TermType.Function, 2 + cs.Count * 5));
                     }
                     else cnt--;
                  }
                  answer.AddRange(TermsConverter.ToTerms(1.ToString()));
                  answer.AddRange(TermsConverter.ToTerms(cnt.ToString()));
                  answer.Add(new Term(Functions.Mat, TermType.Function, 2 + cnt));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetDxfCircles
         if (value.Type == TermType.Function && value.Text == "GetDxfCircles")
         {
            List<Term> answer = new List<Term>();

            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            string filepath = TermsConverter.DecodeText(arg1.Text).Trim('"');
            int indexOfChar = filepath.IndexOf(':');
            if (indexOfChar == -1) filepath = Utilites.CurrentPath(context.FileName) + "\\" + filepath;
            DxfDocument dxf = DxfDocument.Load(filepath);

            string lay = "#";
            double scale = 1;
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#") lay = TermsConverter.DecodeText(value.Items[1].Text).Trim('"');
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[2], context);

            if (dxf.Circles.Count() == 0)
            {
               answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной окружности на заданном слое\""));
               result = Entry.Create(answer.ToArray());
               return true;
            }
            else
            {
               List<Circle> lines = new List<Circle>();
               foreach (Circle item in dxf.Circles)
               {
                  if (lay != "#" && item.Layer.Name == lay) lines.Add(item);
                  else if (lay == "#") lines.Add(item);
                  else continue;
               }
               if (lines.Count == 0)
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной окружности на заданном слое\""));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
               else
               {
                  foreach (Circle item in lines)
                  {
                     answer.AddRange(new TNumber(item.Center.X * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(item.Center.Y * scale).obj.ToTerms());
                     answer.AddRange(new TNumber(2 * item.Radius * scale).obj.ToTerms());
                  }
                  answer.AddRange(TermsConverter.ToTerms(lines.Count.ToString()));
                  answer.AddRange(TermsConverter.ToTerms(3.ToString()));
                  answer.Add(new Term(Functions.Mat, TermType.Function, 2 + lines.Count * 3));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }

         //GetDxfPlines
         if (value.Type == TermType.Function && value.Text == "GetDxfPlines")
         {
            List<Term> answer = new List<Term>();

            Entry arg1 = Computation.Preprocessing(value.Items[0], context);
            string filepath = TermsConverter.DecodeText(arg1.Text).Trim('"');
            int indexOfChar = filepath.IndexOf(':');
            if (indexOfChar == -1) filepath = Utilites.CurrentPath(context.FileName) + "\\" + filepath;
            DxfDocument dxf = DxfDocument.Load(filepath);

            string lay = "#";
            double scale = 1;
            //int isClose = 0;
            if (TermsConverter.DecodeText(value.Items[1].Text).Trim('"') != "#")
            {
               Entry arg = Computation.Preprocessing(value.Items[1], context);
               lay = TermsConverter.DecodeText(arg.Text).Trim('"');
            }
            if (TermsConverter.DecodeText(value.Items[2].Text).Trim('"') != "#") scale = Utilites.Entry2Double(value.Items[2], context);
            //if (TermsConverter.DecodeText(value.Items[3].Text).Trim('"') != "#") isClose = Utilites.Entry2Int(value.Items[3], context);

            if (dxf.LwPolylines.Count() == 0)
            {
               answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной полилинии на заданном слое\""));
               result = Entry.Create(answer.ToArray());
               return true;
            }
            else
            {
               List<LwPolyline> lines = new List<LwPolyline>();
               foreach (LwPolyline item in dxf.LwPolylines)
               {
                  if (lay != "#" && item.Layer.Name == lay) lines.Add(item);
                  else if (lay == "#") lines.Add(item);
                  else continue;
               }
               if (lines.Count == 0)
               {
                  answer.AddRange(TermsConverter.ToTerms("\"Документ не содержит ни одной полилинии на заданном слое\""));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
               else
               {
                  foreach (LwPolyline item in lines)
                  {
                     List<LwPolylineVertex> vxs = item.Vertexes;
                     if (!item.IsClosed)
                        vxs[vxs.Count - 1] = vxs[0];

                     for (int i = 0; i < vxs.Count; i++)
                     {
                        answer.AddRange(new TNumber(vxs[i].Position.X * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(vxs[i].Position.Y * scale).obj.ToTerms());
                     }
                     //foreach (LwPolylineVertex vx in vxs)
                     //{
                     //   answer.AddRange(new TNumber(vx.Position.X * scale).obj.ToTerms());
                     //   answer.AddRange(new TNumber(vx.Position.Y * scale).obj.ToTerms());
                     //}
                     if (item.IsClosed)
                     {
                        answer.AddRange(new TNumber(vxs[0].Position.X * scale).obj.ToTerms());
                        answer.AddRange(new TNumber(vxs[0].Position.Y * scale).obj.ToTerms());
                        answer.AddRange(TermsConverter.ToTerms((vxs.Count + 1).ToString()));
                        answer.AddRange(TermsConverter.ToTerms(2.ToString()));
                        answer.Add(new Term(Functions.Mat, TermType.Function, 4 + vxs.Count * 2));
                     }
                     else
                     {
                        answer.AddRange(TermsConverter.ToTerms(vxs.Count.ToString()));
                        answer.AddRange(TermsConverter.ToTerms(2.ToString()));
                        answer.Add(new Term(Functions.Mat, TermType.Function, 2 + vxs.Count * 2));
                     }
                  }
                  answer.AddRange(TermsConverter.ToTerms(1.ToString()));
                  answer.AddRange(TermsConverter.ToTerms(lines.Count.ToString()));
                  answer.Add(new Term(Functions.Mat, TermType.Function, 2 + lines.Count));
                  result = Entry.Create(answer.ToArray());
                  return true;
               }
            }
         }


         result = null;
         return false;
      }
   }
}
