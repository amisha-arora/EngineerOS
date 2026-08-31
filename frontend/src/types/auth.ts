//These TypeScript types correspond to your ASP.NET Core DTOs.
export type RegisterRequest = {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
};

export type LoginRequest = {
    email: string;
    password: string;
};

export type LoginResponse = {
    accessToken: string;
    refreshToken: string;
};

export type CurrentUser = {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
};