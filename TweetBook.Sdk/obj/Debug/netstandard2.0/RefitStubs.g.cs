﻿// <auto-generated />
using System;
using System.Net.Http;
using System.Collections.Generic;
using TweetBook.Sdk.RefitInternalGenerated;

/* ******** Hey You! *********
 *
 * This is a generated file, and gets rewritten every time you build the
 * project. If you want to edit it, you need to edit the mustache template
 * in the Refit package */

#pragma warning disable
namespace TweetBook.Sdk.RefitInternalGenerated
{
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
    sealed class PreserveAttribute : Attribute
    {

        //
        // Fields
        //
        public bool AllMembers;

        public bool Conditional;
    }
}
#pragma warning restore

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8669 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source.
namespace TweetBook.Sdk
{
    using global::Refit;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Text;
    using global::System.Threading.Tasks;
    using global::TweetBook.Contract.V1.Models;

    /// <inheritdoc />
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [Preserve]
    [global::System.Reflection.Obfuscation(Exclude=true)]
    partial class AutoGeneratedIIdentityApi : IIdentityApi
    {
        /// <inheritdoc />
        public HttpClient Client { get; protected set; }
        readonly IRequestBuilder requestBuilder;

        /// <inheritdoc />
        public AutoGeneratedIIdentityApi(HttpClient client, IRequestBuilder requestBuilder)
        {
            Client = client;
            this.requestBuilder = requestBuilder;
        }

        /// <inheritdoc />
        Task<ReturnUser> IIdentityApi.Register(RegisterModel model)
        {
            var arguments = new object[] { model };
            var func = requestBuilder.BuildRestResultFuncForMethod("Register", new Type[] { typeof(RegisterModel) });
            return (Task<ReturnUser>)func(Client, arguments);
        }

        /// <inheritdoc />
        Task<ReturnUser> IIdentityApi.Login(AuthenticateModel model)
        {
            var arguments = new object[] { model };
            var func = requestBuilder.BuildRestResultFuncForMethod("Login", new Type[] { typeof(AuthenticateModel) });
            return (Task<ReturnUser>)func(Client, arguments);
        }
    }
}

namespace TweetBook.Sdk
{
    using global::Refit;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Text;
    using global::System.Threading.Tasks;
    using global::TweetBook.Contract.V1.Models;

    /// <inheritdoc />
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [Preserve]
    [global::System.Reflection.Obfuscation(Exclude=true)]
    partial class AutoGeneratedITweetBokkApi : ITweetBokkApi
    {
        /// <inheritdoc />
        public HttpClient Client { get; protected set; }
        readonly IRequestBuilder requestBuilder;

        /// <inheritdoc />
        public AutoGeneratedITweetBokkApi(HttpClient client, IRequestBuilder requestBuilder)
        {
            Client = client;
            this.requestBuilder = requestBuilder;
        }

        /// <inheritdoc />
        Task<CreatePostModel> ITweetBokkApi.CreatePost(CreatePostModel model)
        {
            var arguments = new object[] { model };
            var func = requestBuilder.BuildRestResultFuncForMethod("CreatePost", new Type[] { typeof(CreatePostModel) });
            return (Task<CreatePostModel>)func(Client, arguments);
        }

        /// <inheritdoc />
        Task<CreatePostModel> ITweetBokkApi.GetAll()
        {
            var arguments = new object[] {  };
            var func = requestBuilder.BuildRestResultFuncForMethod("GetAll", new Type[] {  });
            return (Task<CreatePostModel>)func(Client, arguments);
        }

        /// <inheritdoc />
        Task<CreatePostModel> ITweetBokkApi.Get(Guid postId)
        {
            var arguments = new object[] { postId };
            var func = requestBuilder.BuildRestResultFuncForMethod("Get", new Type[] { typeof(Guid) });
            return (Task<CreatePostModel>)func(Client, arguments);
        }

        /// <inheritdoc />
        Task<CreatePostModel> ITweetBokkApi.UpdatePost(Guid postId, UpdatePostModel model)
        {
            var arguments = new object[] { postId, model };
            var func = requestBuilder.BuildRestResultFuncForMethod("UpdatePost", new Type[] { typeof(Guid), typeof(UpdatePostModel) });
            return (Task<CreatePostModel>)func(Client, arguments);
        }
    }
}

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning restore CS8669 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source.
