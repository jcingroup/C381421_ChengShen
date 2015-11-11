declare module server {
    interface BaseEntityTable {
        edit_type: number;
        check_del: boolean;
        expland_sub: boolean;
        is_show: boolean;
    }
    interface i_Code {
        code: string;
        langCode: string;
        value: string;
    }
    interface CUYUnit {
        sign: string;
        code: string;
    }
    interface i_Lang extends BaseEntityTable {
        lang: string;
        area: string;
        memo: string;
        isuse: boolean;
        sort: any;
    }
    interface loginField {
        lang: string;
        account: string;
        password: string;
        img_vildate: string;
        rememberme: boolean;

    }    
    interface AspNetRoles extends BaseEntityTable {
        Id: string;
        name: string;
        aspNetUsers: any[];
    }
    interface AspNetUsers extends BaseEntityTable {
        Id: string;
        email: string;
        emailConfirmed: boolean;
        passwordHash: string;
        securityStamp: string;
        phoneNumber: string;
        phoneNumberConfirmed: boolean;
        twoFactorEnabled: boolean;
        lockoutEndDateUtc: Date;
        lockoutEnabled: boolean;
        accessFailedCount: number;
        userName: string;
        department_id: number;
        aspNetRoles: server.AspNetRoles[];
        role_array: any;
    }
    interface ProductType extends BaseEntityTable {
        id: number;
        type_name: string;
        is_second: boolean;
        sort: number;
        memo: string;
        i_Hide: boolean;
        i_InsertUserID: string;
        i_InsertDeptID: number;
        i_InsertDateTime: Date;
        i_UpdateUserID: string;
        i_UpdateDeptID: number;
        i_UpdateDateTime: Date;
        i_Lang: string;
        productData: any[];
    }
    interface ProductData extends BaseEntityTable {
        id: number;
        type_id: number;
        is_second: boolean;
        product_name: string;
        supporting_capacity: string;
        engine: string;
        memo: string;
        sort: number;
        i_Hide: boolean;
        i_InsertUserID: string;
        i_InsertDeptID: number;
        i_InsertDateTime: Date;
        i_UpdateUserID: string;
        i_UpdateDeptID: number;
        i_UpdateDateTime: Date;
        i_Lang: string;
        productType: {
            id: number;
            type_name: string;
            is_second: boolean;
            sort: number;
            memo: string;
            i_Hide: boolean;
            i_InsertUserID: string;
            i_InsertDeptID: number;
            i_InsertDateTime: Date;
            i_UpdateUserID: string;
            i_UpdateDeptID: number;
            i_UpdateDateTime: Date;
            i_Lang: string;
            productData: any[];
        };
    }
    interface Parm extends BaseEntityTable {
        layer: string;
        surfacehandle: string;
        receiveMails: string;
        BccMails: string;
    }
} 