'use client';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
    LayoutDashboard,
    Package,
    Tags,
} from 'lucide-react';

import {
    Sidebar,
    SidebarContent,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuItem,
    SidebarMenuButton,
    SidebarProvider,
    SidebarTrigger,
} from '@/components/ui/sidebar';

import { UserMenu } from '@/components/auth/UserMenu';

const navigation = [
    { name: 'Dashboard', href: '/dashboard', icon: LayoutDashboard },
    { name: 'Products', href: '/products', icon: Package },
    { name: 'Categories', href: '/categories', icon: Tags },
];

interface MainLayoutProps {
    children: React.ReactNode;
}

export function MainLayout({ children }: MainLayoutProps) {
    const pathname = usePathname();

    return (
        <SidebarProvider>
            <div className="flex min-h-screen w-full">
                <Sidebar>
                    <SidebarHeader>
                        <SidebarMenu>
                            <SidebarMenuItem>
                                <Link href="/dashboard" className="flex items-center space-x-2 px-2 py-1">
                                    <div className="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
                                        <Package className="w-5 h-5 text-white" />
                                    </div>
                                    <span className="text-xl font-bold">Hypesoft</span>
                                </Link>
                            </SidebarMenuItem>
                        </SidebarMenu>
                    </SidebarHeader>

                    <SidebarContent>
                        <SidebarGroup>
                            <SidebarGroupLabel>Navigation</SidebarGroupLabel>
                            <SidebarGroupContent>
                                <SidebarMenu>
                                    {navigation.map((item) => {
                                        const isActive = pathname.startsWith(item.href);
                                        return (
                                            <SidebarMenuItem key={item.name}>
                                                <SidebarMenuButton asChild isActive={isActive}>
                                                    <Link href={item.href}>
                                                        <item.icon className="w-4 h-4" />
                                                        <span>{item.name}</span>
                                                    </Link>
                                                </SidebarMenuButton>
                                            </SidebarMenuItem>
                                        );
                                    })}
                                </SidebarMenu>
                            </SidebarGroupContent>
                        </SidebarGroup>
                    </SidebarContent>
                </Sidebar>

                <div className="flex-1 flex flex-col">
                    <header className="border-b bg-background px-6 py-3">
                        <div className="flex items-center justify-between">
                            <div className="flex items-center gap-2">
                                <SidebarTrigger />
                                <h1 className="text-lg font-semibold">
                                    {navigation.find(item => pathname?.startsWith(item.href))?.name || 'Dashboard'}
                                </h1>
                            </div>
                            <div className="ml-auto">
                                <UserMenu />
                            </div>
                        </div>
                    </header>
                    <main className="flex-1 p-6">
                        {children}
                    </main>
                </div>
            </div>
        </SidebarProvider>
    );
}
