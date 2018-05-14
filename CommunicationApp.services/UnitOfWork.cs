using System;
using CommunicationApp.Entity;
using CommunicationApp.Data;

namespace CommunicationApp.Services
{
    public class UnitOfWork : IDisposable
    {
        private DataContext context = new DataContext();
        //Fields
        private RepositoryService<Agent, DataContext> agentRepository;
        private RepositoryService<City, DataContext> cityRepository;
        private RepositoryService<Company, DataContext> companyRepository;
        private RepositoryService<Country, DataContext> countryRepository;
        private RepositoryService<Customer, DataContext> customerRepository;
        private RepositoryService<Event, DataContext> eventRepository;
        private RepositoryService<EventCustomer, DataContext> eventcustomerRepository;
        private RepositoryService<ErrorExceptionLogs, DataContext> errorExceptionLogRepository;
        private RepositoryService<FeedBack, DataContext> feedbackRepository;
        private RepositoryService<Form, DataContext> formRepository;
        private RepositoryService<OfficeLocation, DataContext> officelocationRepository;
        private RepositoryService<PropertyImage, DataContext> propertyimageRepository;
        private RepositoryService<RoleDetail, DataContext> roleDetailRepository;
        private RepositoryService<Role, DataContext> roleRepository;
        private RepositoryService<Property, DataContext> propertyRepository;
        private RepositoryService<State, DataContext> stateRepository;
        private RepositoryService<Tip, DataContext> tipRepository;
        private RepositoryService<UserRole, DataContext> userRoleRepository;
        private RepositoryService<User, DataContext> userRepository;
        private RepositoryService<Notification, DataContext> notificationRepository;
        private RepositoryService<AdminStaff, DataContext> adminstaffRepository;
        private RepositoryService<Views, DataContext> viewsRepository;
        private RepositoryService<Category, DataContext> categoryRepository;
        private RepositoryService<SubCategory, DataContext> subcategoryRepository;
        private RepositoryService<Supplier, DataContext> supplierRepository;
        private RepositoryService<Message, DataContext> messageRepository;
        private RepositoryService<Division, DataContext> divisionRepository;
        private RepositoryService<PdfForm, DataContext> pdfformRepository;

        private RepositoryService<Brokerage, DataContext> brokerageRepository;
        private RepositoryService<BrokerageSevice, DataContext> brokerageServiceRepository;
        private RepositoryService<BrokerageDetail, DataContext> brokerageDetailServiceRepository;
        private RepositoryService<Banner, DataContext> bannerServiceRepository;
        private RepositoryService<MessageImage, DataContext> messageimageRepository;
        private RepositoryService<OfferPrepForm, DataContext> offerPrepFormRepository;
        private RepositoryService<ChattelsTypes, DataContext> chattelsTypeRepository;
        private RepositoryService<ClauseType, DataContext> clauseTypeRepository;
        private RepositoryService<LeaseForm, DataContext> leaseFormRepository;
        private RepositoryService<NewsLetter_Entity, DataContext> newsletterRepository;
        

        //Propertiesssss
        public RepositoryService<Agent, DataContext> AgentRepository
        {
            get
            {

                if (this.agentRepository == null)
                {
                    this.agentRepository = new RepositoryService<Agent, DataContext>(context);
                }
                return agentRepository;
            }
        }
        public RepositoryService<Customer, DataContext> CustomerRepository
        {
            get
            {

                if (this.customerRepository == null)
                {
                    this.customerRepository = new RepositoryService<Customer, DataContext>(context);
                }
                return customerRepository;
            }
        }
        public RepositoryService<City, DataContext> CityRepository
        {
            get
            {
                if (this.cityRepository == null)
                {
                    this.cityRepository = new RepositoryService<City, DataContext>(context);
                }
                return cityRepository;
            }
        }
        public RepositoryService<Company, DataContext> CompanyRepository
        {
            get
            {
                if (this.companyRepository == null)
                {
                    this.companyRepository = new RepositoryService<Company, DataContext>(context);
                }
                return companyRepository;
            }
        }
        public RepositoryService<Country, DataContext> CountryRepository
        {
            get
            {
                if (this.countryRepository == null)
                {
                    this.countryRepository = new RepositoryService<Country, DataContext>(context);
                }
                return countryRepository;
            }
        }
        public RepositoryService<Event, DataContext> EventRepository
        {
            get
            {
                if (this.eventRepository == null)
                {
                    this.eventRepository = new RepositoryService<Event, DataContext>(context);
                }
                return eventRepository;
            }
        }
        public RepositoryService<FeedBack, DataContext> FeedBackRepository
        {
            get
            {
                if (this.feedbackRepository == null)
                {
                    this.feedbackRepository = new RepositoryService<FeedBack, DataContext>(context);
                }
                return feedbackRepository;
            }
        }
        public RepositoryService<ErrorExceptionLogs, DataContext> ErrorExceptionLogsRepository
        {
            get
            {
                if (this.errorExceptionLogRepository == null)
                {
                    this.errorExceptionLogRepository = new RepositoryService<ErrorExceptionLogs, DataContext>(context);
                }
                return errorExceptionLogRepository;
            }
        }
        public RepositoryService<Notification, DataContext> NotificationRepository
        {
            get
            {
                if (this.notificationRepository == null)
                {
                    this.notificationRepository = new RepositoryService<Notification, DataContext>(context);
                }
                return notificationRepository;
            }
        }
        public RepositoryService<Form, DataContext> FormRepository
        {
            get
            {
                if (this.formRepository == null)
                {
                    this.formRepository = new RepositoryService<Form, DataContext>(context);
                }
                return formRepository;
            }
        }
        public RepositoryService<OfficeLocation, DataContext> OfficeLocationRepository
        {
            get
            {
                if (this.officelocationRepository == null)
                {
                    this.officelocationRepository = new RepositoryService<OfficeLocation, DataContext>(context);
                }
                return officelocationRepository;
            }
        }
        public RepositoryService<PropertyImage, DataContext> PropertyImageRepository
        {
            get
            {
                if (this.propertyimageRepository == null)
                {
                    this.propertyimageRepository = new RepositoryService<PropertyImage, DataContext>(context);
                }
                return propertyimageRepository;
            }
        }
        public RepositoryService<Property, DataContext> PropertyRepository
        {
            get
            {
                if (this.propertyRepository == null)
                {
                    this.propertyRepository = new RepositoryService<Property, DataContext>(context);
                }
                return propertyRepository;
            }
        }
        public RepositoryService<RoleDetail, DataContext> RoleDetailRepository
        {
            get
            {
                if (this.roleDetailRepository == null)
                {
                    this.roleDetailRepository = new RepositoryService<RoleDetail, DataContext>(context);
                }
                return roleDetailRepository;
            }
        }
        public RepositoryService<Role, DataContext> RoleRepository
        {
            get
            {
                if (this.roleRepository == null)
                {
                    this.roleRepository = new RepositoryService<Role, DataContext>(context);
                }
                return roleRepository;
            }
        }
        public RepositoryService<Tip, DataContext> TipRepository
        {
            get
            {
                if (this.tipRepository == null)
                {
                    this.tipRepository = new RepositoryService<Tip, DataContext>(context);
                }
                return tipRepository;
            }
        }
        public RepositoryService<State, DataContext> StateRepository
        {
            get
            {
                if (this.stateRepository == null)
                {
                    this.stateRepository = new RepositoryService<State, DataContext>(context);
                }
                return stateRepository;
            }
        }
        public RepositoryService<UserRole, DataContext> UserRoleRepository
        {
            get
            {
                if (this.userRoleRepository == null)
                {
                    this.userRoleRepository = new RepositoryService<UserRole, DataContext>(context);
                }
                return userRoleRepository;
            }
        }
        public RepositoryService<User, DataContext> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new RepositoryService<User, DataContext>(context);
                }
                return userRepository;
            }
        }
        public RepositoryService<EventCustomer, DataContext> EventCustomerRepository
        {
            get
            {
                if (this.eventcustomerRepository == null)
                {
                    this.eventcustomerRepository = new RepositoryService<EventCustomer, DataContext>(context);
                }
                return eventcustomerRepository;
            }
        }
        public RepositoryService<AdminStaff, DataContext> AdminStaffRepository
        {
            get
            {
                if (this.adminstaffRepository == null)
                {
                    this.adminstaffRepository = new RepositoryService<AdminStaff, DataContext>(context);
                }
                return adminstaffRepository;
            }
        }
        public RepositoryService<Views, DataContext> ViewsRepository
        {
            get
            {
                if (this.viewsRepository == null)
                {
                    this.viewsRepository = new RepositoryService<Views, DataContext>(context);
                }
                return viewsRepository;
            }
        }
        public RepositoryService<Category, DataContext> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new RepositoryService<Category, DataContext>(context);
                }
                return categoryRepository;
            }
        }
        public RepositoryService<SubCategory, DataContext> SubCategoryRepository
        {
            get
            {
                if (this.subcategoryRepository == null)
                {
                    this.subcategoryRepository = new RepositoryService<SubCategory, DataContext>(context);
                }
                return subcategoryRepository;
            }
        }
        public RepositoryService<Supplier, DataContext> SupplierRepository
        {
            get
            {
                if (this.supplierRepository == null)
                {
                    this.supplierRepository = new RepositoryService<Supplier, DataContext>(context);
                }
                return supplierRepository;
            }
        }
        public RepositoryService<Message, DataContext> MessageRepository
        {
            get
            {
                if (this.messageRepository == null)
                {
                    this.messageRepository = new RepositoryService<Message, DataContext>(context);
                }
                return messageRepository;
            }
        }
        public RepositoryService<MessageImage, DataContext> MessageImageRepository
        {
            get
            {
                if (this.messageimageRepository == null)
                {
                    this.messageimageRepository = new RepositoryService<MessageImage, DataContext>(context);
                }
                return messageimageRepository;
            }
        }
        public RepositoryService<Division, DataContext> DivisionRepository
        {
            get
            {
                if (this.divisionRepository == null)
                {
                    this.divisionRepository = new RepositoryService<Division, DataContext>(context);
                }
                return divisionRepository;
            }
        }
        public RepositoryService<PdfForm, DataContext> PdfFormRepository
        {
            get
            {
                if (this.pdfformRepository == null)
                {
                    this.pdfformRepository = new RepositoryService<PdfForm, DataContext>(context);
                }
                return pdfformRepository;
            }
        }
        public RepositoryService<Brokerage, DataContext> BrokerageRepository
        {
            get
            {
                if (this.brokerageRepository == null)
                {
                    this.brokerageRepository = new RepositoryService<Brokerage, DataContext>(context);
                }
                return brokerageRepository;
            }
        }
        public RepositoryService<BrokerageSevice, DataContext> BrokerageServiceRepository
        {
            get
            {
                if (this.brokerageServiceRepository == null)
                {
                    this.brokerageServiceRepository = new RepositoryService<BrokerageSevice, DataContext>(context);
                }
                return brokerageServiceRepository;
            }
        }
        public RepositoryService<BrokerageDetail, DataContext> BrokerageDetailServiceRepository
        {
            get
            {
                if (this.brokerageDetailServiceRepository == null)
                {
                    this.brokerageDetailServiceRepository = new RepositoryService<BrokerageDetail, DataContext>(context);
                }
                return brokerageDetailServiceRepository;
            }
        }
        public RepositoryService<Banner, DataContext> BannerServiceRepository
        {
            get
            {
                if (this.bannerServiceRepository == null)
                {
                    this.bannerServiceRepository = new RepositoryService<Banner, DataContext>(context);
                }
                return bannerServiceRepository;
            }
        }

        public RepositoryService<OfferPrepForm, DataContext> OfferPrepFormRepository
        {
            get
            {
                if (this.offerPrepFormRepository == null)
                {
                    this.offerPrepFormRepository = new RepositoryService<OfferPrepForm, DataContext>(context);
                }
                return offerPrepFormRepository;
            }
        }

        public RepositoryService<ChattelsTypes, DataContext> ChattelsTypesRepository
        {
            get
            {
                if (this.chattelsTypeRepository == null)
                {
                    this.chattelsTypeRepository = new RepositoryService<ChattelsTypes, DataContext>(context);
                }
                return chattelsTypeRepository;
            }
        }

        public RepositoryService<ClauseType, DataContext> ClauseTypeRepository
        {
            get
            {
                if (this.clauseTypeRepository == null)
                {
                    this.clauseTypeRepository = new RepositoryService<ClauseType, DataContext>(context);
                }
                return clauseTypeRepository;
            }
        }

        public RepositoryService<LeaseForm, DataContext> LeaseFormRepository
        {
            get
            {
                if (this.leaseFormRepository == null)
                {
                    this.leaseFormRepository = new RepositoryService<LeaseForm, DataContext>(context);
                }
                return leaseFormRepository;
            }
        }

        public RepositoryService<NewsLetter_Entity, DataContext> NewsletterRepository
        {
            get
            {
                if (this.newsletterRepository == null)
                {
                    this.newsletterRepository = new RepositoryService<NewsLetter_Entity, DataContext>(context);
                }
                return newsletterRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
