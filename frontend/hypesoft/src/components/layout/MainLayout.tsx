'use client';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
    LayoutDashboard,
    Package,
    Tags,
    LogOut,
    User
} from 'lucide-react';

import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
    SidebarProvider,
    SidebarTrigger,
} from '@/components/ui/sidebar';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

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

                    <SidebarFooter>
                        <SidebarMenu>
                            <SidebarMenuItem>
                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <SidebarMenuButton>
                                            <Avatar className="w-6 h-6">
                                                <AvatarImage src="" alt="User" />
                                                <AvatarFallback>DU</AvatarFallback>
                                            </Avatar>
                                            <div className="text-left text-sm">
                                                <div className="font-medium">Demo User</div>
                                                <div className="text-xs text-muted-foreground">demo@hypesoft.com</div>
                                            </div>
                                        </SidebarMenuButton>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent side="top" className="w-56">
                                        <DropdownMenuItem>
                                            <User className="mr-2 h-4 w-4" />
                                            Profile
                                        </DropdownMenuItem>
                                        <DropdownMenuItem>
                                            <LogOut className="mr-2 h-4 w-4" />
                                            Sign Out
                                        </DropdownMenuItem>
                                    </DropdownMenuContent>
                                </DropdownMenu>
                            </SidebarMenuItem>
                        </SidebarMenu>
                    </SidebarFooter>
                </Sidebar>

                <div className="flex-1 flex flex-col">
                    {/* Header */}
                    <header className="border-b bg-background px-6 py-3">
                        <div className="flex items-center gap-4">
                            <SidebarTrigger />
                            <h1 className="text-lg font-semibold">
                                {navigation.find(item => pathname?.startsWith(item.href))?.name || 'Dashboard'}
                            </h1>
                        </div>
                    </header>

                    {/* Main content */}
                    <main className="flex-1 p-6">
                        {children}
                    </main>
                </div>
            </div>
        </SidebarProvider>
    );
}
