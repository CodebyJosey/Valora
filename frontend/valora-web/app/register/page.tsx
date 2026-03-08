import { RegisterForm } from "@/components/auth/register-form";

export default function RegisterPage(): React.JSX.Element {
  return (
    <main className="mx-auto flex min-h-[calc(100vh-80px)] w-full max-w-7xl items-center justify-center px-6 py-16 lg:px-8">
      <RegisterForm />
    </main>
  );
}