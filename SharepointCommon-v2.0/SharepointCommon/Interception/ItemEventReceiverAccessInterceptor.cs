﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.JSGrid;
using Microsoft.SharePoint.Utilities;
using SharepointCommon.Attributes;
using SharepointCommon.Common;
using SharepointCommon.Impl;

namespace SharepointCommon.Interception
{
    internal class ItemEventReceiverAccessInterceptor : IInterceptor
    {
        private SPList _list;
        private readonly Hashtable _afterProperties;

        
        public ItemEventReceiverAccessInterceptor(SPList list, Hashtable afterProperties)
        {
            _list = list;
            _afterProperties = afterProperties;
        }

        public void Intercept(IInvocation invocation)
        {
            var propName = invocation.Method.Name.Substring(4);
            var prop = invocation.TargetType.GetProperty(propName);
            
            var fieldAttrs = prop.GetCustomAttributes(typeof(FieldAttribute), true);

            string spPropName;
            if (fieldAttrs.Length != 0)
            {
                spPropName = ((FieldAttribute)fieldAttrs[0]).Name ?? propName;
            }
            else
            {
                spPropName = FieldMapper.TranslateToFieldName(propName);
            }
            propName = spPropName;
            if (invocation.Method.Name.StartsWith("set_"))
            {
                throw new SharepointCommonException("Set operations on AfterProperties mapped item not implemented yet!");
            }

            if (!invocation.Method.Name.StartsWith("get_"))
            {
                Assert.Inconsistent();
            }
            
            switch (invocation.Method.Name)
            {
                case "get_ParentList":
                    var qWeb = new QueryWeb(_list.ParentWeb);
                    invocation.ReturnValue = qWeb.GetById<Item>(_list.ID);
                    return;

                case "get_ConcreteParentList":
                    var qWeb1 = new QueryWeb(_list.ParentWeb);
                    invocation.ReturnValue = CommonHelper.MakeParentList(invocation.TargetType, qWeb1,
                        _list.ID);
                    return;

                case "get_ListItem":
                {
                    invocation.ReturnValue = null;
                    return;
                }

                case "get_Folder":
                {
                    invocation.ReturnValue = null;
                    return;
                }
            }

            if (CommonHelper.IsPropertyNotMapped(prop))
            {
                // skip props with [NotField] attribute
                invocation.Proceed();
                return;
            }

            if (!_afterProperties.ContainsKey(propName))
            {
                var defaultValue = CommonHelper.GetDefaultValue(invocation.Method.ReturnType);
                invocation.ReturnValue = defaultValue;
                return;
            }


            var field = _list.Fields.GetFieldByInternalName(propName);

            var val = EntityMapper.ToEntityField(prop, null, field: field, value: _afterProperties[propName], reloadLookupItem: true);
                
           /* invocation.ReturnValue = _list.Fields.GetFieldByInternalName(propName).Type == SPFieldType.Lookup ||
                                        _list.Fields.GetFieldByInternalName(propName).Type == SPFieldType.User
                ? GetLookupItem(invocation, prop)
                : EntityMapper.ToEntityField(prop, _list, _mappedProperties[propName]);*/

            invocation.ReturnValue = val;
            return;
       
           
        }

        /*

        private object GetLookupItem(IInvocation invocation, PropertyInfo prop)
        {
            var wf = new QueryWeb(_list.ParentWeb);
            var ft = FieldMapper.ToFieldType(invocation.Method);
            var lookupField = _list.Fields.TryGetFieldByStaticName(ft.Name) as SPFieldLookup;

            if (lookupField == null)
            {
                throw new SharepointCommonException(string.Format("cant find '{0}' field in list '{1}'", ft.Name,
                    _list.Title));
            }

            var lookupList = wf.Web.Lists[new Guid(lookupField.LookupList)];
            // Lookup with picker (ilovesharepoint) returns SPFieldLookupValue
            var fieldValue = _mappedProperties[ft.Name];
            
            var lkpValue = fieldValue as SPFieldLookupValue ??
                           new SPFieldLookupValue((string)fieldValue ?? string.Empty);
            if (lkpValue.LookupId == 0) return null;
            var lookupItem = lookupList.GetItemById(lkpValue.LookupId);

            if (typeof (Item).IsAssignableFrom(invocation.Method.ReturnType))
            {
                return lookupItem == null
                ? EntityMapper.ToEntityField(prop, _list, fieldValue)
                : EntityMapper.ToEntity(invocation.Method.ReturnType, lookupItem);
            }
            return EntityMapper.ToEntityField(prop, _list, fieldValue);


        }
        
        private string GetFieldValue(object value, SPField field)
        {
            var result = new StringBuilder();
            if (field is SPFieldLookup)
            {
                var lookupField = field as SPFieldLookup;
                if (!lookupField.AllowMultipleValues)
                {
                    if (value is Item)
                    {
                        var item = value as Item;
                        result.Append(item.Id);
                    }
                    else if (value is User)
                    {
                        var item = value as User;
                        result.Append(item.Id);
                    }

                }
                else
                {
                    var multipleLookupValues = value as IEnumerable;
                    if (multipleLookupValues != null)
                    {
                        var multilookupValueCollection = new SPFieldLookupValueCollection();
                        foreach (var val in multipleLookupValues)
                        {
                            var item = val as Item;
                            if (item != null)
                                multilookupValueCollection.Add(new SPFieldLookupValue(item.Id, string.Empty));
                            else
                            {
                                var user = val as User;
                                if (user != null)
                                    multilookupValueCollection.Add(new SPFieldLookupValue(user.Id, string.Empty));
                            }
                        }
                        result.Append(multilookupValueCollection);
                    }
                }
            }
            else if(field is SPFieldDateTime)
            {
                if (value != null)
                    return SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(value.ToString()));
            }
            else
            {
                result.Append(value);
            }
            return result.ToString();
        }
         */
         
    }
}
