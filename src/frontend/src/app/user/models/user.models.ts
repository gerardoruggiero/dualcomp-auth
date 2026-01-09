export interface UserEntity {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    companyId: string;
    isActive: boolean;
    isEmailValidated: boolean;
    mustChangePassword: boolean;
    isCompanyAdmin: boolean;
    createdAt: string;
    emailValidatedAt?: string;
    // Propiedad calculada en frontend
    fullName?: string;
}

export interface CreateUserCommand {
    firstName: string;
    lastName: string;
    email: string;
    companyId?: string;
    isCompanyAdmin: boolean;
}

export interface UpdateUserCommand {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    companyId?: string;
    isCompanyAdmin: boolean;
}

export interface UserListResult {
    users: UserEntity[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}
