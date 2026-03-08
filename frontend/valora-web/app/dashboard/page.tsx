import { AuthGuard } from "@/components/auth/auth-guard";
import { DashboardHome } from "@/components/dashboard/dashboard-home";

export default function DashboardPage(): React.JSX.Element {
  return (
    <AuthGuard>
      <DashboardHome />
    </AuthGuard>
  );
}