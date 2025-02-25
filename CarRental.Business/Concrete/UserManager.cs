﻿using CarRental.Business.Abstract;
using CarRental.Business.Constants;
using CarRental.Business.Logics;
using CarRental.Business.ValidationRules.FluentValidation;
using CarRental.Core.Aspects.Autofac.Caching;
using CarRental.Core.Aspects.Autofac.Validation;
using CarRental.Core.Entity.Concrete;
using CarRental.Core.Utilities.Business;
using CarRental.Core.Utilities.Results;
using CarRental.DataAccess.Abstract;
using System.Collections.Generic;

namespace CarRental.Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            this._userDal = userDal;
        }

        [ValidationAspect(typeof(UserValidator))]
        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            return new SuccessDataResult<List<OperationClaim>>(_userDal.GetClaims(user));
        }

        public IDataResult<User> GetByMail(string email)
        {
            var result = _userDal.Get(u => u.Email == email);

            if (result == null)
            {
                return new ErrorDataResult<User>(Messages.NotExist("user"));
            }

            return new SuccessDataResult<User>(result);
        }

        //[ValidationAspect(typeof(UserValidator))]
        public IResult Add(User user)
        {
            IResult result = BusinessRules.Run(UserLogics.CheckIfEmailAlreadyExist(_userDal, user.Email));

            if (!result.Success)
            {
                return result;
            }
            
            _userDal.Add(user);

            return new SuccessResult(Messages.SuccesfullyAdded);
        }

        [ValidationAspect(typeof(UserValidator))]
        public IResult Delete(User user)
        {
            _userDal.Delete(user);

            return new SuccessResult(Messages.SuccesfullyDeleted);
        }

        [CacheAspect(10)]
        public IDataResult<List<User>> GetAll()
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll());
        }

        public IDataResult<User> GetByID(int ID)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.ID == ID));
        }

        [ValidationAspect(typeof(UserValidator))]
        public IResult Update(User user)
        {
            IResult result = BusinessRules.Run(UserLogics.CheckIfEmailAlreadyExist(_userDal, user.Email));

            if (!result.Success)
            {
                return result;
            }

            _userDal.Update(user);

            return new SuccessResult(Messages.SuccesfullyUpdated);
        }
    }
}
