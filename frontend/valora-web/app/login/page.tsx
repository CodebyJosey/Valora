import { LoginForm } from "@/components/auth/login-form";

export default function LoginPage(): React.JSX.Element {
  return (
    <main className="mx-auto flex min-h-[calc(100vh-80px)] w-full max-w-7xl items-center justify-center px-6 py-16 lg:px-8">
      <LoginForm />
    </main>
  );
}