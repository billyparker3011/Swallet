######################################################
##                     Project                      ##
######################################################
variable "resource_group_name" {
  type        = string
  description = "(Required) Specifies the Resource Group where the Managed Kubernetes Cluster should exist. Changing this forces a new resource to be created."
}

variable "location" {
  type        = string
  description = "(Required) The location where the Managed Kubernetes Cluster should be created. Changing this forces a new resource to be created."
}

variable "tags" {
  type        = map(string)
  description = "(Optional) A mapping of tags to assign to the resource."
}

######################################################
##             Azure Kubernetes Service             ##
######################################################
variable "kubernetes_cluster_name" {
  type        = string
  description = "(Required) The name of the Managed Kubernetes Cluster to create. Changing this forces a new resource to be created."
}

variable "sku_tier" {
  type        = string
  description = "(Optional) The SKU Tier that should be used for this Kubernetes Cluster. Possible values are Free, and Standard (which includes the Uptime SLA). Defaults to Free."
  default     = "Standard"
}

variable "kubernetes_version" {
  type        = string
  description = "(Optional) Version of Kubernetes specified when creating the AKS managed cluster. If not specified, the latest recommended version will be used at provisioning time (but won't auto-upgrade). AKS does not require an exact patch version to be specified, minor version aliases such as 1.22 are also supported. - The minor version's latest GA patch is automatically chosen in that case. More details can be found in the documentation."
}

variable "private_cluster_enabled" {
  type        = bool
  description = "(Optional) Should this Kubernetes Cluster have its API server only exposed on internal IP addresses? This provides a Private IP Address for the Kubernetes API on the Virtual Network where the Kubernetes Cluster is located. Defaults to false. Changing this forces a new resource to be created."
  default     = true
}

variable "node_resource_group" {
  type        = string
  description = "The auto-generated Resource Group which contains the resources for this Managed Kubernetes Cluster."
}

variable "private_dns_zone_id" {
  type        = string
  description = "(Optional) Either the ID of Private DNS Zone which should be delegated to this Cluster, System to have AKS manage this or None. In case of None you will need to bring your own DNS server and set up resolving, otherwise, the cluster will have issues after provisioning. Changing this forces a new resource to be created."
}

variable "kubernetes_cluster_network_plugin" {
  type        = string
  description = "(Required) Network plugin to use for networking. Currently supported values are azure, kubenet and none. Changing this forces a new resource to be created."
  default     = "azure"
}

variable "aks_default_node_pool_name" {
  type        = string
  description = "(Required) The name which should be used for the default Kubernetes Node Pool. Changing this forces a new resource to be created."
  default     = "default"
}

variable "aks_default_node_pool_vm_size" {
  type        = string
  description = "(Required) The size of the Virtual Machine, such as Standard_DS2_v2."
}

variable "aks_default_node_pool_enable_auto_scaling" {
  type        = bool
  description = "(Optional) Should the Kubernetes Auto Scaler be enabled for this Node Pool?"
  default     = true
}

variable "aks_default_node_pool_max_pods" {
  type        = number
  description = "(Optional) The maximum number of pods that can run on each agent. Changing this forces a new resource to be created."
  default     = 50
}

variable "aks_default_node_pool_min_count" {
  type        = number
  description = "(Optional) The minimum number of nodes which should exist in this Node Pool. If specified this must be between 1 and 1000."
}

variable "aks_default_node_pool_max_count" {
  type        = number
  description = "(Optional) The maximum number of nodes which should exist in this Node Pool. If specified this must be between 1 and 1000."
}

variable "aks_default_node_pool_vnet_subnet_id" {
  type        = string
  description = "(Optional) The ID of a Subnet where the Kubernetes Node Pool should exist. Changing this forces a new resource to be created."
}

variable "aks_ingress_application_gateway_appgw_id" {
  type        = string
  description = "(Optional) The ID of the Application Gateway to integrate with the ingress controller of this Kubernetes Cluster. See this page for further details."
}

variable "aks_identity_type" {
  type        = string
  description = "(Required) Specifies the type of Managed Service Identity that should be configured on this Kubernetes Cluster. Possible values are SystemAssigned or UserAssigned."
  default     = "UserAssigned"
}

variable "aks_identity_ids" {
  type        = set(string)
  description = "(Optional) The ID of the Application Gateway to integrate with the ingress controller of this Kubernetes Cluster. See this page for further details."
}

variable "log_analytics_workspace_id" {
  type        = string
  description = "(Required) The ID of the Log Analytics Workspace which the OMS Agent should send data to."
}

variable "msi_auth_for_monitoring_enabled" {
  type        = bool
  description = "Is managed identity authentication for monitoring enabled?"
  default     = true
}

variable "azure_policy_enabled" {
  type        = bool
  description = "(Optional) Should the Azure Policy Add-On be enabled? For more details please visit Understand Azure Policy for Azure Kubernetes Service"
  default     = true
}

variable "aks_default_node_pool_temporary_name_for_rotation" {
  type        = string
  description = "(Optional) Specifies the name of the temporary node pool used to cycle the default node pool for VM resizing."
  default     = "temporary"
}