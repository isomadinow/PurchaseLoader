PGDMP  6                
    |            ProcurementDb    17.0    17.0     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                           false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                           false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                           false            �           1262    41050    ProcurementDb    DATABASE     �   CREATE DATABASE "ProcurementDb" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE "ProcurementDb";
                     postgres    false            �            1259    41182 	   customers    TABLE     �   CREATE TABLE public.customers (
    id integer NOT NULL,
    organization_name character varying(200),
    inn character varying(100),
    purchase_id integer
);
    DROP TABLE public.customers;
       public         heap r       postgres    false            �            1259    41181    customers_id_seq    SEQUENCE     �   CREATE SEQUENCE public.customers_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.customers_id_seq;
       public               postgres    false    220            �           0    0    customers_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.customers_id_seq OWNED BY public.customers.id;
          public               postgres    false    219            �            1259    41175 	   purchases    TABLE     �   CREATE TABLE public.purchases (
    id integer NOT NULL,
    purchase_number character varying(60) NOT NULL,
    purchase_name character varying(200),
    starting_price numeric(10,3),
    publication_date date
);
    DROP TABLE public.purchases;
       public         heap r       postgres    false            �            1259    41174    purchases_id_seq    SEQUENCE     �   CREATE SEQUENCE public.purchases_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.purchases_id_seq;
       public               postgres    false    218            �           0    0    purchases_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.purchases_id_seq OWNED BY public.purchases.id;
          public               postgres    false    217            ]           2604    41185    customers id    DEFAULT     l   ALTER TABLE ONLY public.customers ALTER COLUMN id SET DEFAULT nextval('public.customers_id_seq'::regclass);
 ;   ALTER TABLE public.customers ALTER COLUMN id DROP DEFAULT;
       public               postgres    false    219    220    220            \           2604    41178    purchases id    DEFAULT     l   ALTER TABLE ONLY public.purchases ALTER COLUMN id SET DEFAULT nextval('public.purchases_id_seq'::regclass);
 ;   ALTER TABLE public.purchases ALTER COLUMN id DROP DEFAULT;
       public               postgres    false    218    217    218            �          0    41182 	   customers 
   TABLE DATA           L   COPY public.customers (id, organization_name, inn, purchase_id) FROM stdin;
    public               postgres    false    220   �       �          0    41175 	   purchases 
   TABLE DATA           i   COPY public.purchases (id, purchase_number, purchase_name, starting_price, publication_date) FROM stdin;
    public               postgres    false    218   Q                   0    0    customers_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.customers_id_seq', 28, true);
          public               postgres    false    219                       0    0    purchases_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.purchases_id_seq', 1, true);
          public               postgres    false    217            a           2606    41187    customers customers_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.customers DROP CONSTRAINT customers_pkey;
       public                 postgres    false    220            _           2606    41180    purchases purchases_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.purchases
    ADD CONSTRAINT purchases_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.purchases DROP CONSTRAINT purchases_pkey;
       public                 postgres    false    218            b           2606    41188 $   customers customers_purchase_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_purchase_id_fkey FOREIGN KEY (purchase_id) REFERENCES public.purchases(id) ON DELETE CASCADE;
 N   ALTER TABLE ONLY public.customers DROP CONSTRAINT customers_purchase_id_fkey;
       public               postgres    false    4703    218    220            �   x  x��Wmr�0����I6��DO��� 0q�m�B2�4W�3�$}��iR��O�V������Кj:Б6��O-�(�
���2���-Dm/:�� >a�@�
��N�\�� �<����~2�A��`f7����ْ�/���;��g���dXt�8�����DO�7A~����� /VZ�3Xh�8�Ç���9: ��Lb��	7��5�T�]�����I~�4����7n��ɛ��n�{�z+����[B�F��a_�[5�A��q-H�%6�ģhG���}�^jW`���bJ����FK�6������{����5�{��q�~g�O_�*G\�Rh�M02�0�^�wu�o�+p�p�;��g2�sa�߬�����5Hg�/�����g��[|*���]�x�;���%m|֭h3��Ti�H'�P�E����Z)��4en2�^{���0LÐ����.m}0�R&Ncֱ�9�{.��o��a��6���ǉp���3�9ark��͑��v�K&v��M�\��t�D̍�m�G�����C�t���t���il��M;�OI��7#q�$�;���ʀ�r��QIW�a�Q��d;�f�[���_�U��      �   v   x����0D�vn 4^d��f�!9R-���k��(�Fo�7�!J�I� R�6��ʕ~�ܹ�ů�;�g��Лͪ�9�O��j��l��RM�]Nc�Џ��ҡt2�K����@q     