using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.BasketModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository,IMapper mapper)
        {
            this._basketRepository = basketRepository;
            this._mapper = mapper;
        }
        public async Task<BasketDTO> CreareOrUpdateBasketAsync(BasketDTO basket)
        {
            CustomerBasket customerBasket = _mapper.Map<BasketDTO , CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(customerBasket);
            return _mapper.Map<CustomerBasket ,BasketDTO>(CreatedOrUpdatedBasket!);
        }

        public async Task<bool> DeleteBasketAsync(string Id)
        {
            return await _basketRepository.DeleteBasketAsync(Id);
        }

        public async Task<BasketDTO> GetBasketAsync(string id)
        {
            var customerBasket = await _basketRepository.GetBasketAsync(id);
            return _mapper.Map<CustomerBasket ,BasketDTO>(customerBasket!);

        }
    }
}
