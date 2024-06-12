######################################################
##                  Resource Group                  ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The Name which should be used for this Resource Group. Changing this forces a new Resource Group to be created."
}

variable "location" {
  type        = string
  description = "(Required) The Name which should be used for this Resource Group. Changing this forces a new Resource Group to be created."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

variable "environment" {
  type        = string
  description = "The environment in which the project is being provisioned (e.g., prod, stage, dev, uat, qa)."
}

######################################################
##                   Linux Web App                  ##
######################################################
variable "linux_web_apps" {
  type = list(object({
    name              = string
    docker_image_name = string
    app_settings      = map(string)
  }))
  description = "(Required) The name which should be used for this Linux Web App. Changing this forces a new Linux Web App to be created."
}

variable "development_service_plan_id" {
  type        = string
  description = "(Required) The ID of the App Service Plan within which to create this Function App."
}

variable "container_registry_managed_identity_client_id" {
  type        = string
  description = "(Optional) The Client ID of the Managed Service Identity to use for connections to the Azure Container Registry."
}

variable "container_registry_use_managed_identity" {
  type        = bool
  description = "(Optional) Should connections for Azure Container Registry use Managed Identity."
  default     = true
}

variable "docker_registry_url" {
  type        = string
  description = "The URL that can be used to log into the container registry."
}

variable "type" {
  type        = string
  description = "(Required) Specifies the type of Managed Service Identity that should be configured on this Linux Web App. Possible values are SystemAssigned, UserAssigned, and SystemAssigned, UserAssigned (to enable both)."
  default     = "UserAssigned"
}

variable "identity_ids" {
  type        = set(string)
  description = "(Optional) A list of User Assigned Managed Identity IDs to be assigned to this Linux Web App."
}

variable "cors_allowed_origins" {
  type        = list(string)
  description = "(Optional) Specifies a list of origins that should be allowed to make cross-origin calls."
  default     = ["*"]
}

variable "support_credentials" {
  type        = bool
  description = "(Optional) Whether CORS requests with credentials are allowed. Defaults to false"
  default     = false
}


