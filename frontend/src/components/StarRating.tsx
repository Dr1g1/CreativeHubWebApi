interface Props {
  value: number;                 
  onChange?: (v: number) => void; 
  size?: number;
}

export default function StarRating({ value, onChange, size = 22 }: Props) {
  const stars = [1, 2, 3, 4, 5];
  const editable = !!onChange;

  return (
    <span style={{ display: "inline-flex", gap: 2 }}>
      {stars.map((s) => (
        <span
          key={s}
          onClick={() => editable && onChange?.(s)}
          style={{
            fontSize: size,
            cursor: editable ? "pointer" : "default",
            color: s <= value ? "var(--burgundy)" : "var(--border)",
            lineHeight: 1,
          }}
        >
          ★
        </span>
      ))}
    </span>
  );
}