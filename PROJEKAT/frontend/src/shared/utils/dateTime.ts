export function toReadableUtc(isoValue: string): string {
  return new Date(isoValue).toUTCString()
}
