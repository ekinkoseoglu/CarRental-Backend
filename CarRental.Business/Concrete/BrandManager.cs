﻿using CarRental.Business.Abstract;
using CarRental.Business.Constants;
using CarRental.Business.Logics;
using CarRental.Business.ValidationRules.FluentValidation;
using CarRental.Core.Aspects.Autofac.Caching;
using CarRental.Core.Aspects.Autofac.Performance;
using CarRental.Core.Aspects.Autofac.Validation;
using CarRental.Core.Utilities.Business;
using CarRental.Core.Utilities.Results;
using CarRental.DataAccess.Abstract;
using CarRental.Entity.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Business.Concrete
{
    public class BrandManager : IBrandService
    {
        private IBrandDal _brandDal;

        public BrandManager(IBrandDal brandDal)
        {
            this._brandDal = brandDal;
        }

        [ValidationAspect(typeof(BrandValidator))]
        [CacheRemoveAspect("IBrandService.Get")]
        public async Task<IResult> Add(Brand brand)
        {
            var result = BusinessRules.Run(BrandLogics.CheckIfBrandAlreadyExist(_brandDal, brand));

            if (!result.Success)
            {
                return result;
            }

            await _brandDal.Add(brand);

            return new SuccessResult(Messages.SuccesfullyAdded);
        }

        [ValidationAspect(typeof(BrandValidator))]
        [CacheRemoveAspect("IBrandService.Get")]
        public async Task<IResult> Delete(Brand brand)
        {
            await _brandDal.Delete(brand);

            return new SuccessResult(Messages.SuccesfullyDeleted);
        }

        [CacheAspect(30)]
        [PerformanceAspect(5)]
        public async Task<IDataResult<List<Brand>>> GetAll()
        {
            return new SuccessDataResult<List<Brand>>(await _brandDal.GetAll());
        }

        [CacheAspect(10)]
        public async Task<IDataResult<Brand>> GetByID(int ID)
        {
            return new SuccessDataResult<Brand>(await _brandDal.Get(b => b.ID == ID));
        }

        [ValidationAspect(typeof(BrandValidator))]
        [CacheRemoveAspect("IBrandService.Get")]
        public async Task<IResult> Update(Brand brand)
        {
            var result = BusinessRules.Run(BrandLogics.CheckIfBrandAlreadyExist(_brandDal, brand));

            if (!result.Success)
            {
                return result;
            }

            await _brandDal.Update(brand);

            return new SuccessResult(Messages.SuccesfullyUpdated);
        }
    }
}
