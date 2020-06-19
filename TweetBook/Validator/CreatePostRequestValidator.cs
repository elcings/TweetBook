using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contract.V1.Models;

namespace TweetBook.Validator
{
    public class CreatePostRequestValidator:AbstractValidator<CreatePostModel>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]*$");
            RuleFor(x => x.Title)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]*$");
        }
    }
}
