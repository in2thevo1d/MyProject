﻿using MyProjectDomain.Result;

namespace MyProjectDomain.Validations
{
    public interface IBaseValidator<in T> where T : class
    {
        BaseResult ValidateOnNull(T model);
    }
}
