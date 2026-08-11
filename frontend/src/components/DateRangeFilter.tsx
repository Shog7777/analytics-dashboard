import { toDateInputValue } from "../utils/format";

export interface DateRange {
  from: string;
  to: string;
}

export function defaultDateRange(daysBack = 30): DateRange {
  const to = new Date();
  const from = new Date();
  from.setDate(from.getDate() - daysBack);
  return { from: toDateInputValue(from), to: toDateInputValue(to) };
}

export function DateRangeFilter({
  value,
  onChange,
}: {
  value: DateRange;
  onChange: (next: DateRange) => void;
}) {
  return (
    <div className="filters-bar">
      <div className="form-group" style={{ marginBottom: 0 }}>
        <label>From</label>
        <input type="date" value={value.from} onChange={(e) => onChange({ ...value, from: e.target.value })} />
      </div>
      <div className="form-group" style={{ marginBottom: 0 }}>
        <label>To</label>
        <input type="date" value={value.to} onChange={(e) => onChange({ ...value, to: e.target.value })} />
      </div>
      <div className="chip-row">
        {[7, 30, 90].map((days) => (
          <button key={days} className="chip" onClick={() => onChange(defaultDateRange(days))} type="button">
            Last {days}d
          </button>
        ))}
      </div>
    </div>
  );
}
