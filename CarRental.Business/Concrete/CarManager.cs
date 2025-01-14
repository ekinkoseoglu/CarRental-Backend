﻿using CarRental.Business.Abstract;
using CarRental.Business.Constants;
using CarRental.Business.Logics;
using CarRental.Business.ValidationRules.FluentValidation;
using CarRental.Core.Aspects.Autofac.Caching;
using CarRental.Core.Aspects.Autofac.Transaction;
using CarRental.Core.Aspects.Autofac.Validation;
using CarRental.Core.Utilities.Business;
using CarRental.Core.Utilities.Results;
using CarRental.DataAccess.Abstract;
using CarRental.Entity.Concrete;
using CarRental.Entity.DTOs;
using System.Collections.Generic;


namespace CarRental.Business.Concrete
{
    public class CarManager : ICarService
    {
        private ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            this._carDal = carDal;
        }

        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Car car)
        {
            IResult result = BusinessRules.Run(
                CarLogics.CheckIfCarNotExists(_carDal, car),
                CarLogics.CheckIfCarCountOfBrandCorrect(_carDal, car.BrandID));

            if (!result.Success)
            {
                return result;
            }

            _carDal.Add(car);

            return new SuccessResult(Messages.SuccesfullyAdded);
        }

        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Delete(Car car)
        {
            _carDal.Delete(car);

            return new SuccessResult(Messages.SuccesfullyDeleted);
        }

        [CacheAspect(10)]
        public IDataResult<List<Car>> GetAll()
        {
            IResult result = BusinessRules.Run(CarLogics.CheckIfSystemAtMaintenanceTime());

            if (!result.Success)
            {
                return new ErrorDataResult<List<Car>>(result.Message);
            }

            return new SuccessDataResult<List<Car>>(_carDal.GetAll());
        }

        [CacheAspect]
        public IDataResult<Car> GetByID(int ID)
        {
            return new SuccessDataResult<Car>(_carDal.Get(c => c.ID == ID));
        }

        public IDataResult<List<CarDetailDTO>> GetDetailedCarsByLocation(int locationID, string uploadPath, string defaultImageFullPath)
        {
            return new SuccessDataResult<List<CarDetailDTO>>(_carDal.GetDetailedCarsByLocation(locationID, uploadPath, defaultImageFullPath));
        }

        public IDataResult<List<CarDetailDTO>> GetCarDetails(string uploadPath, string defaultImageFullPath)
        {
            return new SuccessDataResult<List<CarDetailDTO>>(_carDal.GetCarDetails(uploadPath, defaultImageFullPath));
        }

        public IDataResult<CarDetailDTO> GetCarDetailsByID(int ID, string uploadPath, string defaultImageFullPath)
        {
            return new SuccessDataResult<CarDetailDTO>(_carDal.GetCarDetailsByID(ID, uploadPath, defaultImageFullPath));
        }


        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Update(Car car)
        {
            IResult result = BusinessRules.Run(
                CarLogics.CheckIfCarExists(_carDal, car),
                CarLogics.CheckIfCarCountOfBrandCorrect(_carDal, car.BrandID));

            if (!result.Success)
            {
                return result;
            }

            _carDal.Update(car);

            return new SuccessResult(Messages.SuccesfullyUpdated);
        }

        [TransactionScopeAspect]
        public IResult CheckIfCarExists(int ID)
        {
            var result = GetByID(ID);

            if (result.Data != null)
            {
                return new SuccessResult();
            }

            return new ErrorResult(Messages.NotExist("car"));
        }
    }
}
