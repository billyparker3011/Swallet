######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the Resource Group where this Public IP should exist. Changing this forces a new Public IP to be created."
}

variable "location" {
  type        = string
  description = "(Required) Specifies the supported Azure location where the Public IP should exist. Changing this forces a new resource to be created."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

######################################################
##                 Public IP Address                ##
######################################################
variable "public_ip_name" {
  type        = string
  description = "(Required) Specifies the name of the Public IP. Changing this forces a new Public IP to be created."
}

variable "allocation_method" {
  type        = string
  description = "(Required) Specifies the supported Azure location where the Public IP should exist. Changing this forces a new resource to be created."
  default     = "Static"
}

variable "public_ip_sku" {
  type        = string
  description = "(Optional) The SKU of the Public IP. Accepted values are Basic and Standard. Defaults to Basic. Changing this forces a new resource to be created."
  default     = "Standard"
}

variable "zones" {
  type        = list(string)
  description = "(Optional) A collection containing the availability zone to allocate the Public IP in. Changing this forces a new resource to be created."
  default     = []
}

######################################################
##                Application Gateway               ##
######################################################
variable "application_gateway_name" {
  type        = string
  description = "(Required) The name of the Application Gateway. Changing this forces a new resource to be created."
}

variable "enable_http2" {
  type        = bool
  description = "(Optional) Is HTTP2 enabled on the application gateway resource?"
  default     = true
}

variable "agw_sku_name" {
  type        = string
  description = "(Required) The Name of the SKU to use for this Application Gateway. Possible values are Standard_Small, Standard_Medium, Standard_Large, Standard_v2, WAF_Medium, WAF_Large, and WAF_v2."
  default     = "Standard_v2"
}

variable "agw_sku_tier" {
  type        = string
  description = "(Required) The Tier of the SKU to use for this Application Gateway. Possible values are Standard, Standard_v2, WAF and WAF_v2."
  default     = "Standard_v2"
}

variable "agw_autoscale_configuration_min_capacity" {
  type        = string
  description = "(Required) Minimum capacity for autoscaling. Accepted values are in the range 0 to 100."
  default     = 2
}

variable "agw_autoscale_configuration_max_capacity" {
  type        = string
  description = "(Optional) Maximum capacity for autoscaling. Accepted values are in the range 2 to 125"
  default     = 5
}

variable "agw_gateway_ip_configuration_name" {
  type        = string
  description = "(Required) The Name of this Gateway IP Configuration."
}

variable "agw_subnet_id" {
  type        = string
  description = "(Required) The ID of the Subnet which the Application Gateway should be connected to."
}

variable "agw_frontend_port_name" {
  type        = string
  description = "(Required) The name of the Frontend Port."
}

variable "agw_frontend_port" {
  type        = string
  description = "(Required) The port used for this Frontend Port."
  default     = 80
}

variable "agw_frontend_ip_configuration_name" {
  type        = string
  description = "(Required) The name of the Frontend Port."
}

variable "agw_backend_address_pool_name" {
  type        = string
  description = "(Required) The name of the Backend Address Pool."
}

variable "agw_backend_http_settings_name" {
  type        = string
  description = "(Required) The name of the Backend HTTP Settings Collection."
}

variable "agw_backend_http_settings_cookie_based_affinity" {
  type        = string
  description = "(Required) The name of the Backend HTTP Settings Collection."
  default     = "Disabled"
}

variable "agw_backend_http_settings_port" {
  type        = string
  description = "(Required) The port which should be used for this Backend HTTP Settings Collection."
  default     = "80"
}

variable "agw_backend_http_settings_protocol" {
  type        = string
  description = "(Required) The Protocol which should be used. Possible values are Http and Https."
  default     = "Http"
}

variable "agw_http_listener_name" {
  type        = string
  description = "(Required) The Name of the HTTP Listener."
}

variable "agw_http_listener_protocol" {
  type        = string
  description = "(Required) The Protocol to use for this HTTP Listener. Possible values are Http and Https."
  default     = "Http"
}

variable "agw_request_routing_rule_name" {
  type        = string
  description = "agw_request_routing_rule_name"
}

variable "agw_request_routing_rule_rule_type" {
  type        = string
  description = "(Required) The Type of Routing that should be used for this Rule. Possible values are Basic and PathBasedRouting."
  default     = "Basic"
}

variable "agw_request_routing_rule_priority" {
  type        = number
  description = "(Optional) Rule evaluation order can be dictated by specifying an integer value from 1 to 20000 with 1 being the highest priority and 20000 being the lowest priority."
  default     = 20000
}

variable "agw_ssl_policy_min_protocol_version" {
  type        = string
  description = "(Optional) The minimal TLS version. Possible values are TLSv1_0, TLSv1_1, TLSv1_2 and TLSv1_3."
  default     = "TLSv1_2"
}

variable "agw_ssl_policy_name" {
  type        = string
  description = "(Optional) The Name of the Policy e.g AppGwSslPolicy20170401S. Required if policy_type is set to Predefined. Possible values can change over time and are published here https://docs.microsoft.com/azure/application-gateway/application-gateway-ssl-policy-overview. Not compatible with disabled_protocols."
  default     = "AppGwSslPolicy20170401S"
}

variable "agw_ssl_policy_type" {
  type        = string
  description = "(Optional) The Type of the Policy. Possible values are Predefined, Custom and CustomV2."
  default     = "Predefined"
}































