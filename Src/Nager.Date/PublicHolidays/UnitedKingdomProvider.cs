using Nager.Date.Contract;
using Nager.Date.Extensions;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.Date.PublicHolidays
{
    /// <summary>
    /// United Kingdom
    /// </summary>
    public class UnitedKingdomProvider : IPublicHolidayProvider, ICountyProvider
    {
        private readonly ICatholicProvider _catholicProvider;

        /// <summary>
        /// UnitedKingdomProvider
        /// </summary>
        /// <param name="catholicProvider"></param>
        public UnitedKingdomProvider(ICatholicProvider catholicProvider)
        {
            this._catholicProvider = catholicProvider;
        }

        ///<inheritdoc/>
        public IEnumerable<PublicHoliday> Get(int year)
        {
            var countryCode = CountryCode.GB;

            var firstMondayInAugust = DateSystem.FindDay(year, Month.August, DayOfWeek.Monday, Occurrence.First);
            var lastMondayInAugust = DateSystem.FindLastDay(year, Month.August, DayOfWeek.Monday);

            var easterMonday = this._catholicProvider.EasterMonday("Easter Monday", year, countryCode);
            easterMonday.SetCounties("GB-ENG", "GB-WLS", "GB-NIR");

            var items = new List<PublicHoliday>();

            #region New Year's Day with fallback

            var newYearDay = new DateTime(year, 1, 1);
            if (newYearDay.IsWeekend(countryCode))
            {
                var newYearDayMonday = DateSystem.FindDay(year, Month.January, 1, DayOfWeek.Monday);
                var newYearDayTuesday = DateSystem.FindDay(year, Month.January, 1, DayOfWeek.Tuesday);

                items.Add(new PublicHoliday(newYearDay, "New Year's Day", "New Year's Day", countryCode, null, new string[] { "GB-NIR" }));
                items.Add(new PublicHoliday(newYearDayMonday, "New Year's Day", "New Year's Day", countryCode, null, new string[] { "GB-ENG", "GB-WLS" }));
                items.Add(new PublicHoliday(newYearDayTuesday, "New Year's Day", "New Year's Day", countryCode, null, new string[] { "GB-SCT" }));
            }
            else
            {
                items.Add(new PublicHoliday(newYearDay, "New Year's Day", "New Year's Day", countryCode));
            }

            #endregion

            #region New Year's Day 2 with fallback

            var newYearDay2 = new DateTime(year, 1, 2).Shift(2, 1);
            items.Add(new PublicHoliday(newYearDay2, "New Year's Day", "New Year's Day", countryCode, null, new string[] { "GB-SCT" }));

            #endregion

            items.Add(new PublicHoliday(year, 3, 17, "Saint Patrick's Day", "Saint Patrick's Day", countryCode, null, new string[] { "GB-NIR" }));
            items.Add(this._catholicProvider.GoodFriday("Good Friday", year, countryCode));
            items.Add(easterMonday);
            items.Add(new PublicHoliday(year, 11, 30, "Saint Andrew's Day", "Saint Andrew's Day", countryCode, null, new string[] { "GB-SCT" }));
            items.Add(new PublicHoliday(year, 7, 12, "Battle of the Boyne", "Battle of the Boyne", countryCode, null, new string[] { "GB-NIR" }));
            items.Add(new PublicHoliday(firstMondayInAugust, "Summer Bank Holiday", "Summer Bank Holiday", countryCode, 1971, new string[] { "GB-SCT" }));
            items.Add(new PublicHoliday(lastMondayInAugust, "Summer Bank Holiday", "Summer Bank Holiday", countryCode, 1971, new string[] { "GB-ENG", "GB-WLS", "GB-NIR" }));

            var earlyMayBankHoliday = this.GetEarlyMayBankHoliday(year, countryCode);
            if (earlyMayBankHoliday != null)
            {
                items.Add(earlyMayBankHoliday);
            }

            var springBankHoliday = this.GetSpringBankHoliday(year, countryCode);
            if (springBankHoliday != null)
            {
                items.Add(springBankHoliday);
            }

            var queensPlatinumJubilee = this.GetQueensPlatinumJubilee(year, countryCode);
            if (queensPlatinumJubilee != null)
            {
                items.Add(queensPlatinumJubilee);
            }

            #region Christmas Day with fallback

            var christmasDay = new DateTime(year, 12, 25).Shift(2, 2);
            items.Add(new PublicHoliday(christmasDay, "Christmas Day", "Christmas Day", countryCode));

            #endregion

            #region St. Stephen's Day with fallback

            var sanktStehpenDay = new DateTime(year, 12, 26).Shift(2, 2);
            items.Add(new PublicHoliday(sanktStehpenDay, "Boxing Day", "St. Stephen's Day", countryCode));

            #endregion

            return items.OrderBy(o => o.Date);
        }

        private PublicHoliday GetSpringBankHoliday(int year, CountryCode countryCode)
        {
            var name = "Spring Bank Holiday";

            if (year == 2022)
            {
                //https://www.gov.uk/government/news/extra-bank-holiday-to-mark-the-queens-platinum-jubilee-in-2022
                return new PublicHoliday(year, 6, 2, name, name, countryCode);
            }

            var lastMondayInMay = DateSystem.FindLastDay(year, Month.May, DayOfWeek.Monday);
            return new PublicHoliday(lastMondayInMay, name, name, countryCode, 1971);
        }

        private PublicHoliday GetQueensPlatinumJubilee(int year, CountryCode countryCode)
        {
            if (year == 2022)
            {
                return new PublicHoliday(year, 6, 3, "Queen’s Platinum Jubilee", "Queen’s Platinum Jubilee", countryCode);
            }

            return null;
        }

        private PublicHoliday GetEarlyMayBankHoliday(int year, CountryCode countryCode)
        {
            var holidayName = "Early May Bank Holiday";

            if (year == 2020)
            {
                //https://www.bbc.co.uk/news/uk-48565417
                var secondFridayInMay = DateSystem.FindDay(year, Month.May, DayOfWeek.Friday, Occurrence.Second);
                return new PublicHoliday(secondFridayInMay, holidayName, holidayName, countryCode, 1978);
            }

            var firstMondayInMay = DateSystem.FindDay(year, Month.May, DayOfWeek.Monday, Occurrence.First);
            return new PublicHoliday(firstMondayInMay, holidayName, holidayName, countryCode, 1978);
        }

        ///<inheritdoc/>
        public IDictionary<string, string> GetCounties()
        {
            return new Dictionary<string, string>
            {
                { "GB-NIR", "Northern Ireland" },
                { "GB-SCT", "Scotland" },
                { "GB-ENG", "England" },
                { "GB-WLS", "Wales" },
            };
        }

        ///<inheritdoc/>
        public IEnumerable<string> GetSources()
        {
            return new string[]
            {
                "https://en.wikipedia.org/wiki/Public_holidays_in_the_United_Kingdom",
                "https://de.wikipedia.org/wiki/Feiertage_im_Vereinigten_K%C3%B6nigreich"
            };
        }
    }
}
