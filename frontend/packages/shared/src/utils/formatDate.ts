import moment from 'moment';

/**
 * @deprecated
 */
export function formatNameAndDate(name: string, date: string) {
  const returnDate = date ? moment.utc(new Date(date)).local().format('DD.MM.YYYY HH:mm') : date;
  return name ? `${name} ${returnDate}` : returnDate;
}

/**
 * @deprecated
 */
export function returnDatestringFromDate(date: string, format: string) {
  return moment.utc(date, [format]).format();
}
