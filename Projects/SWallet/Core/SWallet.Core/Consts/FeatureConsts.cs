using SWallet.Core.Models.Features;

namespace SWallet.Core.Consts
{
    public static class FeatureConsts
    {
        public static StaticFeatureModel TransactionsFeature = new()
        {
            FeatureCode = "Feature.Transaction",
            FeatureName = "Transactions",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListDeposit",
                    PermissionName = "List Deposit Transaction"
                },
                new() {
                    PermissionCode = "ListWithdraw",
                    PermissionName = "List Withdraw Transaction"
                },
                new() {
                    PermissionCode = "NewDepositTransaction",
                    PermissionName = "New Deposit Transaction"
                },
                new() {
                    PermissionCode = "NewWithdrawTransaction",
                    PermissionName = "New Withdraw Transaction"
                },
                new() {
                    PermissionCode = "ApproveTransaction",
                    PermissionName = "Approve Transaction"
                },
                new() {
                    PermissionCode = "DenyTransaction",
                    PermissionName = "Deny Transaction"
                }
            }
        };

        public static StaticFeatureModel TicketsFeature = new()
        {
            FeatureCode = "Feature.Ticket",
            FeatureName = "Tickets",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListTicket",
                    PermissionName = "List Ticket"
                },
                new() {
                    PermissionCode = "DeleteTicket",
                    PermissionName = "Delete Ticket"
                }
            }
        };

        public static StaticFeatureModel CustomerFeature = new()
        {
            FeatureCode = "Feature.Customer",
            FeatureName = "Customers",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListCustomer",
                    PermissionName = "List Customer"
                },
                new() {
                    PermissionCode = "ViewCustomerInformation",
                    PermissionName = "View Customer Information"
                },
                new() {
                    PermissionCode = "ViewCustomerBankAccounts",
                    PermissionName = "View Customer Bank Accounts"
                },
                new() {
                    PermissionCode = "ViewCustomerTickets",
                    PermissionName = "View Customer Tickets"
                },
                new() {
                    PermissionCode = "ViewCustomerDepositTransactions",
                    PermissionName = "View Customer Deposit Transaction"
                },
                new() {
                    PermissionCode = "ViewCustomerWithdrawTransactions",
                    PermissionName = "View Customer Withdraw Transaction"
                }
            }
        };

        public static StaticFeatureModel ManagerFeature = new()
        {
            FeatureCode = "Feature.Manager",
            FeatureName = "Managers",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListManager",
                    PermissionName = "List Manager"
                },
                new() {
                    PermissionCode = "NewManager",
                    PermissionName = "Add Manager"
                },
                new() {
                    PermissionCode = "EditManager",
                    PermissionName = "Edit Manager"
                },
                new() {
                    PermissionCode = "ResetPassword",
                    PermissionName = "Reset Password"
                },
                new() {
                    PermissionCode = "ClearMfa",
                    PermissionName = "Clear MFA"
                }
            }
        };

        public static StaticFeatureModel AgentFeature = new()
        {
            FeatureCode = "Feature.Agent",
            FeatureName = "Agents",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListAgent",
                    PermissionName = "List Agent"
                },
                new() {
                    PermissionCode = "NewAgent",
                    PermissionName = "Add Agent"
                },
                new() {
                    PermissionCode = "ViewAgentOuts",
                    PermissionName = "View Outs"
                },
                new() {
                    PermissionCode = "ViewAgentWinlose",
                    PermissionName = "View Winlose"
                },
                new() {
                    PermissionCode = "NewCustomer",
                    PermissionName = "New Customer"
                }
            }
        };

        public static StaticFeatureModel RoleFeature = new()
        {
            FeatureCode = "Feature.System.Role",
            FeatureName = "Role",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListRole",
                    PermissionName = "List Role"
                },
                new() {
                    PermissionCode = "EditRole",
                    PermissionName = "Edit Role"
                },
                new() {
                    PermissionCode = "AssignFeatureAndPermissionRole",
                    PermissionName = "Assign Feature & Permission"
                }
            }
        };

        public static StaticFeatureModel CustomerLevelFeature = new()
        {
            FeatureCode = "Feature.System.CustomerLevel",
            FeatureName = "Customer Levels",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListCustomerLevel",
                    PermissionName = "List Customer Level"
                },
                new() {
                    PermissionCode = "NewCustomerLevel",
                    PermissionName = "Add Customer Level"
                },
                new() {
                    PermissionCode = "EditCustomerLevel",
                    PermissionName = "Edit Customer Level"
                },
                new() {
                    PermissionCode = "DeleteCustomerLevel",
                    PermissionName = "Delete Customer Level"
                }
            }
        };

        public static StaticFeatureModel PaymentMethodFeature = new()
        {
            FeatureCode = "Feature.System.PaymentMethods",
            FeatureName = "Payment Methods",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListPaymentMethod",
                    PermissionName = "List Payment Method"
                },
                new() {
                    PermissionCode = "AddPaymentMethod",
                    PermissionName = "Add Payment Method"
                },
                new() {
                    PermissionCode = "EditPaymentMethod",
                    PermissionName = "Edit Payment Method"
                },
                new() {
                    PermissionCode = "DeletePaymentMethod",
                    PermissionName = "Delete Payment Method"
                }
            }
        };

        public static StaticFeatureModel FeaturesAndPermissionsFeature = new()
        {
            FeatureCode = "Feature.System.FeaturesAndPermissions",
            FeatureName = "Features & Permissions",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListFeatureAndPermission",
                    PermissionName = "List Feature & Permission"
                },
                new() {
                    PermissionCode = "EditFeatureAndPermission",
                    PermissionName = "Edit Feature & Permission"
                }
            }
        };

        public static StaticFeatureModel CloneFeature = new()
        {
            FeatureCode = "Feature.System.Clone",
            FeatureName = "Clone",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "CloneTicket",
                    PermissionName = "Clone Ticket"
                }
            }
        };

        public static StaticFeatureModel BankFeature = new()
        {
            FeatureCode = "Feature.System.Bank",
            FeatureName = "Bank",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListBank",
                    PermissionName = "List Bank"
                },
                new() {
                    PermissionCode = "NewBank",
                    PermissionName = "New Bank"
                },
                new() {
                    PermissionCode = "EditBank",
                    PermissionName = "Edit Bank"
                },
                new() {
                    PermissionCode = "DeleteBank",
                    PermissionName = "Delete Bank"
                }
            }
        };

        public static StaticFeatureModel BankAccountFeature = new()
        {
            FeatureCode = "Feature.System.BankAccount",
            FeatureName = "Bank Account",
            Permissions = new List<StaticPermissionModel>
            {
                new() {
                    PermissionCode = "ListBankAccount",
                    PermissionName = "List Bank Account"
                },
                new() {
                    PermissionCode = "NewBankAccount",
                    PermissionName = "New Bank Account"
                },
                new() {
                    PermissionCode = "EditBankAccount",
                    PermissionName = "Edit Bank Account"
                },
                new() {
                    PermissionCode = "DeleteBankAccount",
                    PermissionName = "Delete Bank Account"
                }
            }
        };
    }
}
