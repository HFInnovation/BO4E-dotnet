using System;
using System.Collections.Generic;
using System.Linq;

using BO4E.COM;
using BO4E.ENUM;
using BO4E.Reporting;

using Itenso.TimePeriod;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using StackExchange.Profiling;

namespace BO4E.Extensions.BusinessObjects.Energiemenge
{
    public static partial class EnergiemengeExtension
    {
        private static List<CompletenessReport.BasicVerbrauch> temp_gaps = new List<CompletenessReport.BasicVerbrauch>();

        /// <summary>
        /// Generate a <see cref="CompletenessReportLight"/> for the given parameters.
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <param name="reference">reference time frame</param>
        /// <param name="wev">Wertermittlungsverfahren</param>
        /// <param name="obiskennzahl">OBIS Kennzahl</param>
        /// <param name="einheit">Mengeneinheit</param>
        /// <returns>the completeness report</returns>
        public static ZeitraumCoverage GetTimeFrameCoverage(this BO4E.BO.Energiemenge em, ITimeRange reference)
        {
            ZeitraumCoverage result;
            using (MiniProfiler.Current.Step("create List of ZeitraumCoverage + find the coverage"))
            {
                result = new ZeitraumCoverage
                {
                    Startdatum = new DateTimeOffset(DateTime.SpecifyKind(reference.Start, DateTimeKind.Utc)),
                    Enddatum = new DateTimeOffset(DateTime.SpecifyKind(reference.End, DateTimeKind.Utc)),
                    Coverage = GetCoverage(em, reference),
                };
            }

            if (em.Energieverbrauch != null && em.Energieverbrauch.Count > 0)
            {
                using (MiniProfiler.Current.Step("Setting aggregated gaps"))
                {
                    var nonNullValues = new TimePeriodCollection(em.Energieverbrauch.Select(v => new TimeRange(v.Startdatum, v.Enddatum)));
                    ITimeRange limits;
                    if (result != null && result.Startdatum.HasValue && result.Enddatum.HasValue)
                    {
                        limits = new TimeRange(result.Startdatum.Value.UtcDateTime, result.Enddatum.Value.UtcDateTime);
                    }
                    else
                    {
                        limits = null;
                    }
                    var gaps = (new TimeGapCalculator<TimeRange>()).GetGaps(nonNullValues, limits: limits);
                    temp_gaps.AddRange(gaps.Select(gap => new CompletenessReport.BasicVerbrauch()
                    {
                        Startdatum = gap.Start,
                        Enddatum = gap.End,
                        Wert = null
                    }).ToList());

                }
            }

            return result;
        }

        /// <summary>
        /// Generate a <see cref="CompletenessReportLight"/> for the given refenrence time frame <paramref name="reference"/>
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <param name="reference">reference time frame</param>
        /// <returns></returns>
        public static CompletenessReportLight GetCompletenessReportLight(this BO4E.BO.Energiemenge em, ITimeRange reference)
        {
            var combis = em.GetWevObisMeCombinations();
            if (combis.Count != 1)
            {
                string errorMessage;
                decimal? coverage;
                if (combis.Count == 0)
                {
                    errorMessage = $"Cannot use autoconfigured method because there are no values.";
                    coverage = 0;
                }
                else
                {
                    errorMessage = $"Cannot use autoconfigured method because there are {combis.Count}>1 distinct (wertermittlungsverfahren, obis, einheit) tuple present: {JsonConvert.SerializeObject(combis, new StringEnumConverter())}";
                    coverage = null;
                }
                return new CompletenessReportLight()
                {
                    LokationsId = em.LokationsId,
                    TimeFrameList = new List<ZeitraumCoverage>(){ new ZeitraumCoverage()
                    {
                        Startdatum = new DateTimeOffset(reference.Start),
                        Enddatum = new DateTimeOffset(reference.End),
                        Coverage = coverage
                    }
                    },
                    ErrorMessage = errorMessage
                };
            }
            var combi = combis.First();
            return em.GetCompletenessReportLight(reference, combi.Item1, combi.Item2, combi.Item3);
        }

        /// <summary>
        /// <see cref="GetCompletenessReport(BO.Energiemenge, ITimeRange, Wertermittlungsverfahren, string, Mengeneinheit)"/>
        /// for pure Energiemengen within their own time range.
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <returns><see cref="GetCompletenessReport(BO.Energiemenge, ITimeRange, Wertermittlungsverfahren, string, Mengeneinheit)"/></returns>
        public static CompletenessReportLight GetCompletenessReportLight(this BO4E.BO.Energiemenge em)
        {
            if (!em.IsPure())
            {
                throw new ArgumentException("The provided Energiemenge is not pure. Please use overloaded method GetCompletenessReport(... , wertermittlungsverfahren, obiskennzahl, mengeneinheit).");
            }
            Verbrauch v;
            try
            {
                v = em.Energieverbrauch.First<Verbrauch>();
            }
            catch (InvalidOperationException)
            {
                return new CompletenessReportLight()
                {
                    LokationsId = em.LokationsId,
                    ErrorMessage = "energieverbrauch is empty",
                    TimeFrameList = new List<ZeitraumCoverage>() { new ZeitraumCoverage() { } }
                };
            }
            return em.GetCompletenessReportLight(em.GetTimeRange(), v.Wertermittlungsverfahren, v.Obiskennzahl, v.Einheit);
        }


        /// <summary>
        /// Generate a <see cref="CompletenessReportLight"/> for the given parameters.
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <param name="reference">reference time frame</param>
        /// <param name="wev">Wertermittlungsverfahren</param>
        /// <param name="obiskennzahl">OBIS Kennzahl</param>
        /// <param name="einheit">Mengeneinheit</param>
        /// <returns>the completeness report</returns>
        public static CompletenessReportLight GetCompletenessReportLight(this BO4E.BO.Energiemenge em, ITimeRange reference, Wertermittlungsverfahren wev, string obiskennzahl, Mengeneinheit einheit)
        {
            if (!em.IsPure())
            {
                throw new ArgumentException("The provided Energiemenge is not pure. Please use overloaded method GetCompletenessReportLight(... , wertermittlungsverfahren, obiskennzahl, mengeneinheit).");
            }
            Verbrauch v;
            try
            {
                v = em.Energieverbrauch.First<Verbrauch>();
            }
            catch (InvalidOperationException ex)
            {
                return new CompletenessReportLight()
                {
                    LokationsId = em.LokationsId,
                    ErrorMessage = "energieverbrauch is empty",
                    TimeFrameList = new List<ZeitraumCoverage>() { new ZeitraumCoverage() { } }
                };
            }
            temp_gaps = new List<CompletenessReport.BasicVerbrauch>();
            var result = new CompletenessReportLight
            {
                LokationsId = em.LokationsId,
                Einheit = einheit,
                wertermittlungsverfahren = wev,
                Obiskennzahl = obiskennzahl,
                TimeFrameList = new List<ZeitraumCoverage>() { em.GetTimeFrameCoverage(reference) },
                Gaps = temp_gaps,
            };
            if (em.IsPure(checkUserProperties: true))
            {
                try
                {
                    foreach (var kvp in em.Energieverbrauch.Where(x => x.UserProperties != null).SelectMany(x => x.UserProperties))
                    {
                        if (result.UserProperties == null)
                        {
                            result.UserProperties = new Dictionary<string, JToken>();
                        }
                        if (!result.UserProperties.ContainsKey(kvp.Key))
                        {
                            result.UserProperties.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // ok, there's no Verbrauch with user properties.
                }
            }
            return result;
        }

        /// <summary>
        /// creates a dictionary of completeness reports for a given list of reference time ranges.
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <param name="ranges">list of ranges for which the completeness reports are generated</param>
        /// <param name="useParallelExecution">set true to internally use parallel linq</param>
        /// <returns></returns>
        public static IDictionary<ITimeRange, ZeitraumCoverage> GetTimeFrameCoverage(this BO.Energiemenge em, IEnumerable<ITimeRange> ranges, bool useParallelExecution = false)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException(nameof(ranges), "list of time ranges must not be null");
            }
            if (ranges.Count() > 0)
            {
                if (useParallelExecution)
                {
                    return ranges.AsParallel().ToDictionary(r => r, r => GetTimeFrameCoverage(em, r));
                }
                else
                {
                    return ranges.ToDictionary(r => r, r => GetTimeFrameCoverage(em, r));
                }
            }
            else
            {
                return new Dictionary<ITimeRange, ZeitraumCoverage>();
            }
        }

        /// <summary>
        /// Get Daily Completeness Reports for <paramref name="overallTimeRange"/>. The magic is, that it takes DST into account!
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <param name="overallTimeRange">overall time frame. Beginning and end must have same hour/minute/second</param>
        /// <param name="useParallelExecution">set true to internally use parallel linq</param>
        /// <returns></returns>
        public static CompletenessReportLight GetDailyCompletenessReportsLight(this BO4E.BO.Energiemenge em, ITimeRange overallTimeRange, bool useParallelExecution = false)
        {
            var slices = GetLocalDailySlices(overallTimeRange);
            temp_gaps = new List<CompletenessReport.BasicVerbrauch>();

            if (!em.IsPure())
            {
                throw new ArgumentException("The provided Energiemenge is not pure. Please use overloaded method GetCompletenessReportLight(... , wertermittlungsverfahren, obiskennzahl, mengeneinheit).");
            }
            Verbrauch v;
            try
            {
                v = em.Energieverbrauch.First<Verbrauch>();
            }
            catch (InvalidOperationException)
            {
                return new CompletenessReportLight()
                {
                    LokationsId = em.LokationsId,
                    ErrorMessage = "energieverbrauch is empty",
                };
            }
            var result = new CompletenessReportLight
            {
                LokationsId = em.LokationsId,
                Einheit = v.Einheit,
                wertermittlungsverfahren = v.Wertermittlungsverfahren,
                Obiskennzahl = v.Obiskennzahl,
                TimeFrameList = em.GetTimeFrameCoverage(slices, useParallelExecution).Values,
                Gaps = temp_gaps,
            };
            if (em.IsPure(checkUserProperties: true))
            {
                try
                {
                    foreach (var kvp in em.Energieverbrauch.Where(x => x.UserProperties != null).SelectMany(x => x.UserProperties))
                    {
                        if (result.UserProperties == null)
                        {
                            result.UserProperties = new Dictionary<string, JToken>();
                        }
                        if (!result.UserProperties.ContainsKey(kvp.Key))
                        {
                            result.UserProperties.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // ok, there's no Verbrauch with user properties.
                }
            }
            return result;
        }

        /// <summary>
        /// Get Monthly Completeness Reports for <paramref name="overallTimeRange"/>. 
        /// </summary>
        /// <param name="em">Energiemenge</param>
        /// <param name="overallTimeRange"></param>
        /// <param name="useParallelExecution">set true to internally use parallel linq</param>
        /// <returns></returns>
        public static CompletenessReportLight GetMonthlyCompletenessReportLights(this BO4E.BO.Energiemenge em, ITimeRange overallTimeRange, bool useParallelExecution = false)
        {
            var slices = GetLocalMonthlySlices(overallTimeRange);
            temp_gaps = new List<CompletenessReport.BasicVerbrauch>();
            if (!em.IsPure())
            {
                throw new ArgumentException("The provided Energiemenge is not pure. Please use overloaded method GetCompletenessReportLight(... , wertermittlungsverfahren, obiskennzahl, mengeneinheit).");
            }
            Verbrauch v;
            try
            {
                v = em.Energieverbrauch.First<Verbrauch>();
            }
            catch (InvalidOperationException)
            {
                return new CompletenessReportLight()
                {
                    LokationsId = em.LokationsId,
                    ErrorMessage = "energieverbrauch is empty",
                };
            }
            var result = new CompletenessReportLight
            {
                LokationsId = em.LokationsId,
                Einheit = v.Einheit,
                wertermittlungsverfahren = v.Wertermittlungsverfahren,
                Obiskennzahl = v.Obiskennzahl,
                TimeFrameList = em.GetTimeFrameCoverage(slices, useParallelExecution).Values,
                Gaps = temp_gaps,
            };

            if (em.IsPure(checkUserProperties: true))
            {
                try
                {
                    foreach (var kvp in em.Energieverbrauch.Where(x => x.UserProperties != null).SelectMany(x => x.UserProperties))
                    {
                        if (result.UserProperties == null)
                        {
                            result.UserProperties = new Dictionary<string, JToken>();
                        }
                        if (!result.UserProperties.ContainsKey(kvp.Key))
                        {
                            result.UserProperties.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // ok, there's no Verbrauch with user properties.
                }
            }
            return result;
        }
    }
}
