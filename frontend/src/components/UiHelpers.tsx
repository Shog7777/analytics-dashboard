export function LoadingState({ label = "Loading..." }: { label?: string }) {
  return <div className="loading-state">{label}</div>;
}

export function EmptyState({ label = "Nothing to show yet." }: { label?: string }) {
  return <div className="empty-state">{label}</div>;
}

export function ErrorBanner({ message }: { message: string }) {
  return <div className="error-banner">{message}</div>;
}
