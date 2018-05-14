using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web.Routing;
public static class HtmlExtensions
{

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]

    public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText = "", string ClassName = "label-required")
    {
        return LabelHelper(html, ModelMetadata.FromLambdaExpression(expression, html.ViewData), ExpressionHelper.GetExpressionText(expression), labelText, ClassName);
    }
    private static MvcHtmlString LabelHelper(HtmlHelper html,
        ModelMetadata metadata, string htmlFieldName, string labelText, string ClassName)
    {
        if (string.IsNullOrEmpty(labelText))
        {
            labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        }
        if (string.IsNullOrEmpty(labelText))
        {
            return MvcHtmlString.Empty;
        }
        bool isRequired = true;
        //if (metadata.ContainerType != null)
        //{
        //    isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
        //                    .GetCustomAttributes(typeof(RequiredAttribute), false)
        //                    .Length == 1;
        //}
        TagBuilder tag = new TagBuilder("label");
        tag.Attributes.Add(
            "for",
            TagBuilder.CreateSanitizedId(
                html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)
            )
        );

        if (isRequired) { tag.Attributes.Add("class", ClassName); }
        //tag.SetInnerText(labelText);

        var output=tag.ToString(TagRenderMode.StartTag);
        output += labelText;
        if (isRequired)
        {
            var asteriskTag = new TagBuilder("span");
            asteriskTag.Attributes.Add("class", "required");
            asteriskTag.SetInnerText("*");
            output += asteriskTag.ToString(TagRenderMode.Normal);
        }
        output += tag.ToString(TagRenderMode.EndTag);

        //<label class="control-label col-md-2" for="Position">Position</label>

        
        return MvcHtmlString.Create(output);
    }

    public static MvcHtmlString LabelForTextRequired(this HtmlHelper helper, string htmlFieldName, string labelText = "", string ClassName = "label-required")
    {
        return LabelHelperText(helper,htmlFieldName, labelText, ClassName);
    }
    private static MvcHtmlString LabelHelperText(this HtmlHelper helper, string htmlFieldName, string labelText, string ClassName)
    {
        if (string.IsNullOrEmpty(labelText))
        {
            labelText = htmlFieldName.Split('.').Last();
        }
        if (string.IsNullOrEmpty(labelText))
        {
            return MvcHtmlString.Empty;
        }
        bool isRequired = true;
       
        TagBuilder tag = new TagBuilder("label");
        tag.Attributes.Add(
            "for",
            TagBuilder.CreateSanitizedId(htmlFieldName)
        );

        if (isRequired) { tag.Attributes.Add("class", ClassName); }
        //tag.SetInnerText(labelText);

        var output = tag.ToString(TagRenderMode.StartTag);
        output += labelText;
        if (isRequired)
        {
            var asteriskTag = new TagBuilder("span");
            asteriskTag.Attributes.Add("class", "required");
            asteriskTag.SetInnerText("*");
            output += asteriskTag.ToString(TagRenderMode.Normal);
        }
        output += tag.ToString(TagRenderMode.EndTag);

        return MvcHtmlString.Create(output);
    }
    //This overload is extension method that accepts two parameters i.e. name and Ienumerable list of values to populate.
    public static MvcHtmlString DropdownListCustom(this HtmlHelper helper, string name, string defaultValue, IEnumerable<SelectListItem> list)
    {
        //This method in turns calls below overload.
        return DropdownListCustom(helper, name, defaultValue, list, null);
    }

    //This overload is extension method accepts name, list and htmlAttributes as parameters.
    public static MvcHtmlString DropdownListCustom(this HtmlHelper helper, string name, string defaultValue, IEnumerable<SelectListItem> list, object htmlAttributes)
    {
        //Creating a select element using TagBuilder class which will create a dropdown.
        TagBuilder dropdown = new TagBuilder("select");
        //Setting the name and id attribute with name parameter passed to this method.
        dropdown.Attributes.Add("name", name);
        dropdown.Attributes.Add("id", name);

        //Created StringBuilder object to store option data fetched oen by one from list.
        StringBuilder options = new StringBuilder();
        //Iterated over the IEnumerable list.
        foreach (var item in list)
        {
            //Each option represents a value in dropdown. For each element in the list, option element is created and appended to the stringBuilder object.

            if (item.Value == defaultValue)
            {
                options = options.Append("<option selected value='" + item.Value + "'>" + item.Text + "</option>");
            }
            else
            {
                options = options.Append("<option value='" + item.Value + "'>" + item.Text + "</option>");
            }

        }
        //assigned all the options to the dropdown using innerHTML property.
        dropdown.InnerHtml = options.ToString();
        //Assigning the attributes passed as a htmlAttributes object.
        dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));
        //Returning the entire select or dropdown control in HTMLString format.
        return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
    }
  
   
    }

