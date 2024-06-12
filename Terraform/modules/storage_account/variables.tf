######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) The name of the resource group in which to create the storage account. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
}

variable "site" {
  type        = string
  description = "(Required) Determine if the region is primary or backup for a Disaster Recovery solution."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

######################################################
##                  Storage Account                 ##
######################################################
variable "storage_account_name" {
  type        = string
  description = "(Required) Specifies the name of the storage account. Only lowercase Alphanumeric characters allowed. Changing this forces a new resource to be created. This must be unique across the entire Azure service, not just within the resource group."
}

variable "account_tier" {
  type        = string
  description = "(Required) Defines the Tier to use for this storage account. Valid options are Standard and Premium. For BlockBlobStorage and FileStorage accounts only Premium is valid. Changing this forces a new resource to be created."
}

variable "account_replication_type" {
  type        = string
  description = "(Required) Defines the type of replication to use for this storage account. Valid options are LRS, GRS, RAGRS, ZRS, GZRS and RAGZRS."
}

variable "min_tls_version" {
  type        = string
  description = "(Optional) The minimum supported TLS version for the storage account. Possible values are TLS1_0, TLS1_1, and TLS1_2. Defaults to TLS1_2 for new storage accounts."
  default     = "TLS1_2"
}

variable "allowed_origins" {
  type        = list(string)
  description = "(Required) A list of headers that are allowed to be a part of the cross-origin request."
  default     = ["*"]
}

variable "allowed_methods" {
  type        = list(string)
  description = "(Required) A list of HTTP methods that are allowed to be executed by the origin. Valid options are DELETE, GET, HEAD, MERGE, POST, OPTIONS, PUT or PATCH."
  default     = ["DELETE", "GET", "HEAD", "MERGE", "POST", "OPTIONS", "PUT", "PATCH"]
}

variable "allowed_headers" {
  type        = list(string)
  description = "(Required) A list of origin domains that will be allowed by CORS."
  default     = ["*"]
}

variable "exposed_headers" {
  type        = list(string)
  description = "(Required) A list of response headers that are exposed to CORS clients."
  default     = ["*"]
}

variable "max_age_in_seconds" {
  type        = number
  description = "(Required) The number of seconds the client should cache a preflight response."
  default     = 0
}

variable "blob_properties_delete_retention_policy_days" {
  type        = number
  description = "(Optional) Specifies the number of days that the blob should be retained, between 1 and 365 days. Defaults to 7."
  default     = 7
}

variable "blob_properties_container_delete_retention_policy_days" {
  type        = number
  description = "(Optional) Specifies the number of days that the container should be retained, between 1 and 365 days. Defaults to 7."
  default     = 14
}

variable "blob_properties_versioning_enabled" {
  type        = bool
  description = "(Optional) Is versioning enabled? Default to false"
}

######################################################
##             Storage Account Container            ##
######################################################
variable "containers" {
  type        = list(string)
  description = "(Required) The name of the Container which should be created within the Storage Account. Changing this forces a new resource to be created."
}

variable "container_access_type" {
  type        = string
  description = "(Optional) The Access Level configured for this Container. Possible values are blob, container or private. Defaults to private."
  default     = "blob"
}

######################################################
##                   Storage Blob                   ##
######################################################
variable "blobs" {
  type = list(object({
    path         = string
    container    = string
    content_type = string
  }))
  description = "(Required) The name of the storage blob. Must be unique within the storage container the blob is located. Changing this forces a new resource to be created."
  default     = []
}

variable "type" {
  type        = string
  description = "(Required) The type of the storage blob to be created. Possible values are Append, Block or Page. Changing this forces a new resource to be created."
  default     = "Block"
}

variable "source_uri" {
  type        = string
  description = "(Optional) The URI of an existing blob, or a file in the Azure File service, to use as the source contents for the blob to be created. Changing this forces a new resource to be created. This field cannot be specified for Append blobs and cannot be specified if source or source_content is specified."
  default     = ""
}

variable "SAS" {
  type    = string
  default = ""
}