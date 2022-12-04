using SMath.Manager;

using SMath.Math.Numeric;
using System;
using System.Collections.Generic;
using SMath.Math;
using System.IO;

namespace AcadToSMath
{ 
   static class Utilites
   {
      public static string _exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
      public static string _exeDirectory = GlobalProfile.ApplicationPath;
      public static string _exeFileName = Path.GetFileName(_exePath);

      public static string CurrentPath(string StoreFileName)
      {
         if (String.IsNullOrEmpty(StoreFileName))
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);

         if (!StoreFileName.Contains(Path.DirectorySeparatorChar.ToString()))
            return Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

         return Path.GetDirectoryName(StoreFileName);
      }

      static public List<string> BlueScale { get; } = new List<string>() {"\"#08306B\"",
                                                                          "\"#08519C\"",
                                                                          "\"#2171B5\"",
                                                                          "\"#4292C6\"",
                                                                          "\"#6BAED6\"",
                                                                          "\"#9ECAE1\"",
                                                                          "\"#C6DBEF\"",
                                                                          "\"#DEEBF7\"",
                                                                          "\"#F7FBFF\""};
      static public List<string> RedScale { get; } = new List<string>() {"\"#800026\"",
                                                                         "\"#BD0026\"",
                                                                         "\"#E31A1C\"",
                                                                         "\"#FC4E2A\"",
                                                                         "\"#FD8D3C\"",
                                                                         "\"#FEB24C\"",
                                                                         "\"#FED976\"",
                                                                         "\"#FFEDA0\"",
                                                                         "\"#FFFFCC\""};

      internal static double[] EntryVec2Arr(Entry vector, Store store)
      {
         TNumber num = Computation.NumericCalculation(vector, store);
         TMatrix matrix = (TMatrix)num.obj;
         double[] res = new double[matrix.Length()];
         //for (int i = 0; i < matrix.unit.Length; i++)
         //{
         //   res[i] = matrix.unit[i, 0].obj.ToDouble();
         //}

         string source = matrix.ToString(15, 15, FractionsType.Decimal, false);
         source = source.Trim(new char[] { 'm', 'a', 't', '(', ')' });
         source = source.Replace("*10^{", "E");
         source = source.Replace("}", "");
         string[] src = source.Split(',');

         int n = matrix.Length();
         for (int i = 0; i < n; i++)
         {
            //if (src[i].IndexOf('^') >= 0) res[i] = 0;
            res[i] = Convert.ToDouble(src[i]);
         }
         return res;
      }

      internal static double[,] EntryMat2Arr(Entry mat, Store store)
      {
         TNumber num = Computation.NumericCalculation(mat, store);
         TMatrix matrix = (TMatrix)num.obj;
         double[,] res = new double[matrix.unit.GetLength(0), matrix.unit.GetLength(1)];
         for (int i = 0; i < matrix.unit.GetLength(0); i++)
         {
            for (int j = 0; j < matrix.unit.GetLength(1); j++)
            {
               res[i, j] = matrix.unit[i, j].obj.ToDouble();
            }
         }
         return res;
      }

      internal static double Entry2Double(Entry prime, Store store)
      {
         TNumber num = Computation.NumericCalculation(prime, store);
         return num.obj.ToDouble();
      }

      internal static int Entry2Int(Entry prime, Store store)
      {
         TNumber num = Computation.NumericCalculation(prime, store);
         return (int)num.obj.ToDouble();
      }

      internal static List<string> SelectColors(List<double> values)
      {
         List<double> source = new List<double>(values);
         source.Sort();
         double start = source[0];
         double end = source[source.Count - 1];
         double stepCold = start/ 9;
         double stepHot = end / 9;
         double[] cold = new double[10];
         double[] hot = new double[10];
         cold[0] = start;
         hot[0] = end;
         for (int i = 1; i < 9; i++)
         {
            cold[i] = start - i * stepCold;
            hot[i] = end - i * stepHot;
         }
         cold[9] = 0;
         hot[9] = 0;

         List<string> res = new List<string>();
         foreach (double item in values)
         {
            for (int i = 0; i < 9; i++)
            {
               if (item >= cold[i] && item <= cold[i + 1])
               {
                  res.Add(BlueScale[i]);
                  break;
               }
               else if (item <= hot[i] && item >= hot[i + 1])
               {
                  res.Add(RedScale[i]);
                  break;
               }
            }
         }

         return res;
      }
   }
}
